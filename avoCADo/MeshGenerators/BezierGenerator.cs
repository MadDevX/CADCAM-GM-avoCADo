using avoCADo.Architecture;
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

        private bool _showEdges = false;
        private bool _showVirtualControlPoints = false;
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
        public IList<DrawCall> DrawCalls => new List<DrawCall> { new DrawCall(0, _indices.Length, DrawCallShaderType.Default, RenderConstants.CURVE_SIZE) };

        public bool ShowVirtualControlPoints
        {
            get => _showVirtualControlPoints;
            set
            {
                if(value != _showVirtualControlPoints)
                {
                    _showVirtualControlPoints = value;
                    DataChangedWrapper();
                }
            }
        }

        private INode _parentNode;
        public ICurve Curve { get; }

        private uint[] _curveIndices = new uint[0];
        private float[] _curveVertexData = new float[0];

        private uint[] _edgeIndices = new uint[0];
        private float[] _edgeVertexData = new float[0];

        private uint[] _virtualEdgeIndices = new uint[0];
        private float[] _virtualEdgeVertexData = new float[0];

        private uint[] _indices = new uint[0];
        private float[] _vertexData = new float[0];

        private bool _isInitialized = false;

        private int _subdivisions = 250;

        private float _maxDistanceSum;
        private int AdjustedSubdivisions => ((int)(_maxDistanceSum / 25.0f) + 1) * _subdivisions;

        public BezierGenerator(ICurve curve)
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
            Curve.Refresh();
            UpdateDistanceSum(_parentNode.Children);
            SourceDataChanged();
            SourceDataChangedEdges();
            SourceDataChangedVirtualEdges();
            CheckCombineArrays();
            //UpdateVirtualPoints();
            OnParametersChanged?.Invoke();
        }

        private void SourceDataChanged()
        {
            var nodes = _parentNode.Children;

            var subdivisions = AdjustedSubdivisions;
            var segments = Curve.Segments;
            var fullDivisions = segments * subdivisions;
            if (fullDivisions * 3 != _curveVertexData.Length)
            {
                _curveIndices = new uint[Math.Max(0, fullDivisions * 2 - 2)];
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
                SetVertex(_curveVertexData, Curve.GetVertex(t), i);
            });

        }

        private void SourceDataChangedEdges()
        {
            var nodes = Curve.ControlPoints;
            if (nodes.Count * 3 != _edgeVertexData.Length)
            {
                _edgeIndices = new uint[Math.Max(0, nodes.Count * 2 - 2)];
                _edgeVertexData = new float[3 * nodes.Count];
            }


            for (uint i = 0; i < _edgeIndices.Length; i++) _edgeIndices[i] = (uint)(_curveIndices.Length/2+1 + ((i+1)/2));

            for (int i = 0; i < nodes.Count; i++)
            {
                SetVertex(_edgeVertexData, nodes[i].Transform.WorldPosition, i);
            }

        }

        private void SourceDataChangedVirtualEdges()
        {
            var vCtrlPoints = Curve as IVirtualControlPoints;
            if (vCtrlPoints != null)
            {
                var nodes = vCtrlPoints.VirtualControlPoints;
                if (nodes.Count * 3 != _virtualEdgeVertexData.Length)
                {
                    _virtualEdgeIndices = new uint[Math.Max(0, nodes.Count * 2 - 2)];
                    _virtualEdgeVertexData = new float[3 * nodes.Count];
                }


                var offset = (_curveIndices.Length / 2 + 1) + (_edgeIndices.Length / 2 + 1);
                for (uint i = 0; i < _virtualEdgeIndices.Length; i++) _virtualEdgeIndices[i] = (uint)(offset + ((i + 1) / 2));

                for (int i = 0; i < nodes.Count; i++)
                {
                    SetVertex(_virtualEdgeVertexData, nodes[i], i);
                }
            }
        }

        private void CheckCombineArrays()
        {
            if (ShowEdges)
            {
                if (_vertexData.Length != _curveVertexData.Length + _edgeVertexData.Length + _virtualEdgeVertexData.Length)
                {
                    _vertexData = new float[_curveVertexData.Length + _edgeVertexData.Length + _virtualEdgeVertexData.Length];
                    _indices = new uint[_curveIndices.Length + _edgeIndices.Length + _virtualEdgeIndices.Length];
                }
                Array.Copy(_curveVertexData, 0, _vertexData, 0, _curveVertexData.Length);
                Array.Copy(_edgeVertexData, 0, _vertexData, _curveVertexData.Length, _edgeVertexData.Length);
                Array.Copy(_virtualEdgeVertexData, 0, _vertexData, _curveVertexData.Length + _edgeVertexData.Length, _virtualEdgeVertexData.Length);
                Array.Copy(_curveIndices, 0, _indices, 0, _curveIndices.Length);
                Array.Copy(_edgeIndices, 0, _indices, _curveIndices.Length, _edgeIndices.Length);
                Array.Copy(_virtualEdgeIndices, 0, _indices, _curveIndices.Length + _edgeIndices.Length, _virtualEdgeIndices.Length);
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

        private void SetVertex(float[] vertexArray, Vector3 vect, int vertexIndex)
        {
            vertexArray[3 * vertexIndex + 0] = vect.X;
            vertexArray[3 * vertexIndex + 1] = vect.Y;
            vertexArray[3 * vertexIndex + 2] = vect.Z;
        }
        #endregion
    }
}
