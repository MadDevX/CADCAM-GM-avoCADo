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

        private INode _parentNode;
        private uint[] _indices = new uint[0];
        private float[] _vertexData = new float[0];

        private bool _isInitialized = false;

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
        }

        private void SourceDataChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            SourceDataChanged();
        }

        private void SourceDataChanged()
        {
            var nodes = _parentNode.Children;

            if (nodes.Count != _indices.Length)
            {
                _indices = new uint[nodes.Count];
                _vertexData = new float[3 * nodes.Count];
            }


            for (uint i = 0; i < nodes.Count; i++) _indices[i] = i;

            for (int i = 0; i < nodes.Count; i++)
            {
                var worldPos = nodes[i].Transform.WorldPosition;
                _vertexData[3 * i + 0] = worldPos.X;
                _vertexData[3 * i + 1] = worldPos.Y;
                _vertexData[3 * i + 2] = worldPos.Z;
            }

            OnParametersChanged?.Invoke();
        }

        public uint[] GetIndices()
        {
            return _indices;
        }

        public float[] GetVertices()
        {
            return _vertexData;
        }
    }
}
