using OpenTK;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo
{
    public class BezierGenerator : IMeshGenerator, IDependent<INode>
    {
        public event Action OnParametersChanged;

        public bool ShowEdges { get; set; } = true;

        private INode _parentNode;

        private uint[] _curveIndices = new uint[0];
        private float[] _curveVertexData = new float[0];

        private uint[] _edgeIndices = new uint[0];
        private float[] _edgeVertexData = new float[0];

        private uint[] _indices = new uint[0];
        private float[] _vertexData = new float[0];

        private bool _isInitialized = false;

        private int _subdivisions = 100;

        private float _distanceSum;
        private int AdjustedSubdivisions => ((int)(_distanceSum / 50.0f) + 1) * _subdivisions;

        public void Initialize(INode node)
        {
            if (_isInitialized == false)
            {
                _parentNode = node;
                _parentNode.PropertyChanged += SourceDataChanged;
                _parentNode.Children.CollectionChanged += SourceDataChanged;
                _isInitialized = true;
            }
            else throw new InvalidOperationException("Tried to initialize existing BezierGenerator instance twice!");
        }

        public void Dispose()
        {
            _parentNode.PropertyChanged -= SourceDataChanged;
            _parentNode.Children.CollectionChanged -= SourceDataChanged;
        }

        private void SourceDataChanged(object sender, PropertyChangedEventArgs e)
        {
            SourceDataChanged();
            SourceDataChangedEdges();
            CheckCombineArrays();
            OnParametersChanged?.Invoke();
        }

        private void SourceDataChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            SourceDataChanged();
            SourceDataChangedEdges();
            CheckCombineArrays();
            OnParametersChanged?.Invoke();
        }

        #region SourceDataChanged
        private void SourceDataChanged()
        {
            var nodes = _parentNode.Children;
            UpdateDistanceSum(nodes);

            var subdivisions = AdjustedSubdivisions;
            var fn = nodes.Count < 2? 0 : ((nodes.Count + 1) / 3);
            if (fn * subdivisions != _curveIndices.Length)
            {
                _curveIndices = new uint[fn * subdivisions];
                _curveVertexData = new float[3 * fn * subdivisions];
            }


            for (uint i = 0; i < _curveIndices.Length; i++) _curveIndices[i] = i;

            for (int i = 0; i < nodes.Count; i+=3)
            {
                if (i + 1 == nodes.Count) break;
                else if (i + 2 == nodes.Count) CalcBezier1(i);
                else if (i + 3 == nodes.Count) CalcBezier2(i);
                else if (i + 4 <= nodes.Count) CalcBezier3(i);
                else throw new Exception("indexing error");
            }

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
            if(ShowEdges)
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
        #endregion


        #region Bezier Calculations

        private void CalcBezier1(int i)
        {
            var subdivisions = AdjustedSubdivisions;
            var nodes = _parentNode.Children;
            Vector3 a = nodes[i].Transform.WorldPosition;
            Vector3 b = nodes[i + 1].Transform.WorldPosition;
            for (int j = 0; j < subdivisions; j++)
            {
                var vect = Bezier1(a, b, j / (float)(subdivisions - 1));
                SetVertex(vect, i / 3 * subdivisions + j);
            }
        }
        private void CalcBezier2(int i)
        {
            var subdivisions = AdjustedSubdivisions;
            var nodes = _parentNode.Children;
            Vector3 a = nodes[i].Transform.WorldPosition;
            Vector3 b = nodes[i + 1].Transform.WorldPosition;
            Vector3 c = nodes[i + 2].Transform.WorldPosition;
            for (int j = 0; j < subdivisions; j++)
            {
                var vect = Bezier2(a, b, c, j / (float)(subdivisions - 1));
                SetVertex(vect, i / 3 * subdivisions + j);
            }
        }

        private void CalcBezier3(int i)
        {
            var subdivisions = AdjustedSubdivisions;
            var nodes = _parentNode.Children;
            Vector3 a = nodes[i].Transform.WorldPosition;
            Vector3 b = nodes[i + 1].Transform.WorldPosition;
            Vector3 c = nodes[i + 2].Transform.WorldPosition;
            Vector3 d = nodes[i + 3].Transform.WorldPosition;
            for (int j = 0; j < subdivisions; j++)
            {
                var vect = Bezier3(a, b, c, d, j / (float)(subdivisions - 1));
                SetVertex(vect, i / 3 * subdivisions + j);
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
            _distanceSum = 0.0f;
            for (int i = 0; i < nodes.Count - 1; i++)
            {
                _distanceSum += (nodes[i].Transform.WorldPosition - nodes[i + 1].Transform.WorldPosition).Length;
            }
        }

        private void SetVertex(Vector3 vect, int vertexIndex)
        {
            _curveVertexData[3 * vertexIndex + 0] = vect.X;
            _curveVertexData[3 * vertexIndex + 1] = vect.Y;
            _curveVertexData[3 * vertexIndex + 2] = vect.Z;
        }
        #endregion
    }
}
