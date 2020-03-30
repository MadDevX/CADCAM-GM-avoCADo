using OpenTK;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo.MeshGenerators
{
    public class BezierGeneratorGeometry : IMeshGenerator, IDependent<INode>
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
        public IList<DrawCall> DrawCalls => new List<DrawCall> { new DrawCall(0, _indices.Length, DrawCallShaderType.Default) };

        private INode _parentNode;
        private ICurve Curve { get; }

        private uint[] _indices = new uint[0];
        private float[] _vertexData = new float[0];

        private bool _isInitialized = false;

        public BezierGeneratorGeometry(ICurve curve)
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
            SourceDataChanged();
            OnParametersChanged?.Invoke();
        }

        private void SourceDataChanged()
        {
            var nodes = Curve.ControlPoints;


            if (nodes.Count * 3 != _vertexData.Length)
            {
                _indices = new uint[Math.Max(0, nodes.Count * 2 - 2)];
                _vertexData = new float[3 * nodes.Count];
            }

            for(int i = 0; i < _indices.Length; i++)
            {
                _indices[i] = (uint)((i + 1) / 2);
            }

            for(int i = 0; i < nodes.Count; i++)
            {
                SetVertex(_vertexData, nodes[i].Transform.WorldPosition, i);
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

        private void SetVertex(float[] vertexArray, Vector3 vect, int vertexIndex)
        {
            vertexArray[3 * vertexIndex + 0] = vect.X;
            vertexArray[3 * vertexIndex + 1] = vect.Y;
            vertexArray[3 * vertexIndex + 2] = vect.Z;
        }
        #endregion
    }
}
