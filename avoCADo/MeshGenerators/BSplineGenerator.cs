using OpenTK;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo
{
    /// <summary>
    /// Obsolete to delete xD
    /// </summary>
    public class BSplineGenerator : IMeshGenerator, IDependent<INode>
    {
        public event Action OnParametersChanged;

        private bool _showEdges = false;
        public bool ShowEdges
        {
            get => _showEdges;
            set
            {
                if (value != _showEdges)
                {
                    _showEdges = value;
                    SourceDataChangedWrapper();
                }
            }
        }
        public IList<DrawCall> DrawCalls => new List<DrawCall> { new DrawCall(0, _indices.Length, DrawCallShaderType.Default, RenderConstants.CURVE_SIZE) };

        private INode _parentNode;
        private List<Vector3> _bernsteinPoints = new List<Vector3>();

        private uint[] _curveIndices = new uint[0];
        private float[] _curveVertexData = new float[0];

        private uint[] _edgeIndices = new uint[0];
        private float[] _edgeVertexData = new float[0];

        private uint[] _indices = new uint[0];
        private float[] _vertexData = new float[0];

        private bool _isInitialized = false;

        private int _subdivisions = 200;

        private float _maxDistanceSum;
        private int AdjustedSubdivisions => ((int)(_maxDistanceSum / 25.0f) + 1) * _subdivisions;

        public void Initialize(INode node)
        {
            if (_isInitialized == false)
            {
                _parentNode = node;
                _parentNode.PropertyChanged += SourceDataChanged;
                _parentNode.Children.CollectionChanged += SourceDataChanged;
                _isInitialized = true;
            }
            else throw new InvalidOperationException("Tried to initialize existing BSplineGenerator instance twice!");
        }

        public void Dispose()
        {
            _parentNode.PropertyChanged -= SourceDataChanged;
            _parentNode.Children.CollectionChanged -= SourceDataChanged;
        }

        public void RefreshDataPreRender()
        {
        }

        private void SourceDataChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            SourceDataChangedWrapper();
        }

        private void SourceDataChanged(object sender, PropertyChangedEventArgs e)
        {
            SourceDataChangedWrapper();
        }

        private void SourceDataChangedWrapper()
        {
            CalculateBernsteinControlPoints();
            SourceDataChanged();
            //SourceDataChangedEdges();
            CheckCombineArrays();
            OnParametersChanged?.Invoke();
        }

        private void CalculateBernsteinControlPoints()
        {
            _bernsteinPoints.Clear();
            var deBoorPoints = _parentNode.Children;
            for(int i = 0; i < _parentNode.Children.Count - 3; i++)
            {
                AddBernstein(_bernsteinPoints, deBoorPoints[i + 0].Transform.WorldPosition,
                                              deBoorPoints[i + 1].Transform.WorldPosition,
                                              deBoorPoints[i + 2].Transform.WorldPosition,
                                              deBoorPoints[i + 3].Transform.WorldPosition,
                                              i == _parentNode.Children.Count - 4
                                              );
            }
            
        }

        private void AddBernstein(List<Vector3> bernsteinPointsList, Vector3 a, Vector3 b, Vector3 c, Vector3 d, bool addLast)
        {
            var oneThird = 1.0f / 3.0f;
            var twoThirds = 2.0f / 3.0f;

            var firstMid = Vector3.Lerp(a, b, twoThirds);
            var secondMid = Vector3.Lerp(b, c, oneThird);
            var thirdMid = Vector3.Lerp(b, c, twoThirds);
            var fourthMid = Vector3.Lerp(c, d, oneThird);
            bernsteinPointsList.Add(Vector3.Lerp(firstMid, secondMid, 0.5f));
            bernsteinPointsList.Add(secondMid);
            bernsteinPointsList.Add(thirdMid);
            if(addLast) bernsteinPointsList.Add(Vector3.Lerp(thirdMid, fourthMid, 0.5f));
        }

        private void SourceDataChanged()
        {
            var nodes = _bernsteinPoints;
            UpdateDistanceSum(_parentNode.Children);

            var subdivisions = AdjustedSubdivisions;
            var fn = nodes.Count < 2 ? 0 : ((nodes.Count + 1) / 3);
            if (fn == 0) return;
            if (fn * subdivisions != _curveIndices.Length)
            {
                _curveIndices = new uint[fn * subdivisions * 2 - 2];
                _curveVertexData = new float[3 * fn * subdivisions];
            }

            Parallel.For(0, _curveIndices.Length, (i) => _curveIndices[i] = (uint)((i+1)/2));

            Parallel.For(0, nodes.Count / 3 + 1, (i3) =>
            {
                int i = i3 * 3;
                if (i + 1 == nodes.Count) return;
                else if (i + 2 == nodes.Count) { }
                else if (i + 3 == nodes.Count) { }
                else if (i + 4 <= nodes.Count) CalcBezier3(i);
            });

        }

        private void SourceDataChangedEdges()
        {
            var nodes = _parentNode.Children;

            if (nodes.Count != _edgeIndices.Length)
            {
                _edgeIndices = new uint[nodes.Count];
                _edgeVertexData = new float[3 * nodes.Count];
            }


            for (uint i = 0; i < nodes.Count; i++) _edgeIndices[i] = (uint)_curveIndices.Length + i;

            for (int i = nodes.Count - 1; i >= 0; i--)
            {
                var invI = nodes.Count - 1 - i;
                var worldPos = nodes[i].Transform.WorldPosition;
                _edgeVertexData[3 * invI + 0] = worldPos.X;
                _edgeVertexData[3 * invI + 1] = worldPos.Y;
                _edgeVertexData[3 * invI + 2] = worldPos.Z;
            }

        }

        private void CheckCombineArrays()
        {
            if (ShowEdges)
            {
                if (_vertexData.Length != _curveVertexData.Length + _edgeVertexData.Length)
                {
                    _vertexData = new float[_curveVertexData.Length + _edgeVertexData.Length];
                    _indices = new uint[_curveIndices.Length + _edgeIndices.Length];
                }
                Array.Copy(_curveVertexData, 0, _vertexData, 0, _curveVertexData.Length);
                Array.Copy(_edgeVertexData, 0, _vertexData, _curveVertexData.Length, _edgeVertexData.Length);
                Array.Copy(_curveIndices, 0, _indices, 0, _curveIndices.Length);
                Array.Copy(_edgeIndices, 0, _indices, _curveIndices.Length, _edgeIndices.Length);
            }
            else
            {
                _vertexData = _curveVertexData;
                _indices = _curveIndices;
            }
        }

        #region Bezier
        private void CalcBezier3(int i)
        {
            var subdivisions = AdjustedSubdivisions;
            var nodes = _bernsteinPoints;
            Vector3 a = nodes[i]    ;//.Transform.WorldPosition;
            Vector3 b = nodes[i + 1];//.Transform.WorldPosition;
            Vector3 c = nodes[i + 2];//.Transform.WorldPosition;
            Vector3 d = nodes[i + 3];//.Transform.WorldPosition;
            for (int j = 0; j < subdivisions; j++)
            {
                var vect = Bezier3(a, b, c, d, j / (float)(subdivisions - 1));
                VBOUtility.SetVertex(_curveVertexData, vect, i / 3 * subdivisions + j);
            }
        }

        private Vector3 Bezier1(Vector3 a, Vector3 b, float t)
        {
            return Vector3.Lerp(a, b, t);
        }

        private Vector3 Bezier2(Vector3 a, Vector3 b, Vector3 c, float t)
        {
            return Vector3.Lerp(Bezier1(a, b, t), Bezier1(b, c, t), t);
        }

        private Vector3 Bezier3(Vector3 a, Vector3 b, Vector3 c, Vector3 d, float t)
        {
            return Vector3.Lerp(Bezier2(a, b, c, t), Bezier2(b, c, d, t), t);
        }
        #endregion

        #region Getters

        public uint[] GetIndices()
        {
            return _indices;
        }

        public float[] GetVertices()
        {
            return _vertexData;
        }

        #endregion

        #region Utility
        private void UpdateDistanceSum(ObservableCollection<INode> nodes)
        {
            _maxDistanceSum = 0.0f;
            for (int i = 0; i < nodes.Count - 1; i += 3)
            {
                var curDist = 0.0f;
                if (i + 1 < nodes.Count) curDist += (nodes[i].Transform.WorldPosition - nodes[i + 1].Transform.WorldPosition).Length;
                if (i + 2 < nodes.Count) curDist += (nodes[i + 1].Transform.WorldPosition - nodes[i + 2].Transform.WorldPosition).Length;
                if (i + 3 < nodes.Count) curDist += (nodes[i + 2].Transform.WorldPosition - nodes[i + 3].Transform.WorldPosition).Length;

                if (curDist > _maxDistanceSum)
                {
                    _maxDistanceSum = curDist;
                }
            }
        }
        #endregion
    }
}
