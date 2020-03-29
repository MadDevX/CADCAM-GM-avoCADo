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
    public class BezierGeneratorNew : IMeshGenerator, IDependent<INode>
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
                    DataChangedWrapper();
                }
            }
        }

        private INode _parentNode;
        private ICurve Curve { get; }

        private uint[] _curveIndices = new uint[0];
        private float[] _curveVertexData = new float[0];

        private uint[] _edgeIndices = new uint[0];
        private float[] _edgeVertexData = new float[0];

        private uint[] _indices = new uint[0];
        private float[] _vertexData = new float[0];

        private bool _isInitialized = false;

        private int _subdivisions = 250;

        private float _maxDistanceSum;
        private int AdjustedSubdivisions => ((int)(_maxDistanceSum / 25.0f) + 1) * _subdivisions;

        public BezierGeneratorNew(ICurve curve)
        {
            Curve = curve;
        }

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
            DataChangedWrapper();
        }

        private void SourceDataChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            DataChangedWrapper();
        }

        #region SourceDataChanged

        private void DataChangedWrapper()
        {
            UpdateDistanceSum(_parentNode.Children);
            SourceDataChanged();
            SourceDataChangedEdges();
            CheckCombineArrays();
            OnParametersChanged?.Invoke();
        }

        private void SourceDataChanged()
        {
            var nodes = _parentNode.Children;

            var subdivisions = AdjustedSubdivisions;
            var fn = nodes.Count < 2 ? 0 : ((nodes.Count + 1) / 3);
            var fullDivisions = fn * subdivisions;
            if (fullDivisions != _curveIndices.Length)
            {
                _curveIndices = new uint[fullDivisions * 2 - 2];
                _curveVertexData = new float[3 * fullDivisions];
            }

            Parallel.For(0, _curveIndices.Length, (i) =>
            {
                _curveIndices[i] = (uint)((i+1)/2);
            });

            var paramRange = Curve.ParameterRange;
            var diff = paramRange.Y - paramRange.X;
            if (diff == 0.0f) return;
            Parallel.For(0, fullDivisions, (i) =>
            {
                var t = ((float)i / (float)(fullDivisions - 1) * diff) + paramRange.X;
                SetVertex(Curve.GetVertex(t), i);
            });

        }

        private void SourceDataChangedEdges()
        {
            var nodes = Curve.ControlPoints;

            if (nodes.Count != _edgeIndices.Length)
            {
                _edgeIndices = new uint[nodes.Count];
                _edgeVertexData = new float[3 * nodes.Count];
            }


            for (uint i = 0; i < nodes.Count; i++) _edgeIndices[i] = (uint)_curveIndices.Length + i;

            for (int i = nodes.Count - 1; i >= 0; i--)
            {
                var invI = nodes.Count - 1 - i;
                SetEdgeVertex(nodes[i].Transform.WorldPosition, invI);
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

        private void SetVertex(Vector3 vect, int vertexIndex)
        {
            _curveVertexData[3 * vertexIndex + 0] = vect.X;
            _curveVertexData[3 * vertexIndex + 1] = vect.Y;
            _curveVertexData[3 * vertexIndex + 2] = vect.Z;
        }

        private void SetEdgeVertex(Vector3 vect, int vertexIndex)
        {
            _edgeVertexData[3 * vertexIndex + 0] = vect.X;
            _edgeVertexData[3 * vertexIndex + 1] = vect.Y;
            _edgeVertexData[3 * vertexIndex + 2] = vect.Z;
        }
        #endregion
    }
}
