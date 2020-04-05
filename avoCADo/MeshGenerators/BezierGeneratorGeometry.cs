using avoCADo.Architecture;
using OpenTK;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo
{
    public class BezierGeneratorGeometry : IMeshGenerator, IDependent<INode>
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

        public bool ShowVirtualControlPoints
        {
            get => _showVirtualControlPoints;
            set
            {
                if (value != _showVirtualControlPoints)
                {
                    _showVirtualControlPoints = value;
                    DataChangedWrapper();
                }
            }
        }

        public IList<DrawCall> DrawCalls 
        { 
            get
            {
                if (ShowEdges)
                {
                    return new List<DrawCall>
                    {
                        new DrawCall(0, _curveIndices.Length, DrawCallShaderType.Curve),
                        new DrawCall(_curveIndices.Length, _edgeIndices.Length, DrawCallShaderType.Default),
                        new DrawCall(_curveIndices.Length + _edgeIndices.Length, _virtualEdgeIndices.Length, DrawCallShaderType.Default)
                    };
                }
                else
                {
                    return new List<DrawCall>
                    {
                        new DrawCall(0, _curveIndices.Length, DrawCallShaderType.Curve)
                    };
                }
            }
        }

        private INode _parentNode;
        public ICurve Curve { get; }

        private uint[] _indices = new uint[0];
        private float[] _vertexData = new float[0];

        private uint[] _curveIndices = new uint[0];
        private float[] _curveVertexData = new float[0];

        private uint[] _edgeIndices = new uint[0];
        private float[] _edgeVertexData = new float[0];

        private uint[] _virtualEdgeIndices = new uint[0];
        private float[] _virtualEdgeVertexData = new float[0];

        private bool _isInitialized = false;

        private SelectionManager _selectionManager;

        public BezierGeneratorGeometry(ICurve curve)
        {
            Curve = curve;
            _selectionManager = NodeSelection.Manager;
            _selectionManager.OnSelectionChanged += HandleSelectionChanged;
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
            DisposeVirtualNodes();
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
            SourceDataChangedEdges();
            SourceDataChangedVirtualEdges();
            CheckCombineArrays();
            UpdateVirtualPoints();
            UpdateDrawCalls();
            OnParametersChanged?.Invoke();
        }

        private void UpdateDrawCalls()
        {
            if(ShowEdges && DrawCalls.Count != 3)
            {
                DrawCalls.Clear();
                DrawCalls.Add(new DrawCall(0, _curveIndices.Length, DrawCallShaderType.Curve));
                DrawCalls.Add(new DrawCall(_curveIndices.Length, _edgeIndices.Length, DrawCallShaderType.Default));
                DrawCalls.Add(new DrawCall(_curveIndices.Length + _edgeIndices.Length, _virtualEdgeIndices.Length, DrawCallShaderType.Default));
            }
            else if(DrawCalls.Count != 1)
            {
                DrawCalls.Clear();
                DrawCalls.Add(new DrawCall(0, _curveIndices.Length, DrawCallShaderType.Curve));
            }
        }

        private void SourceDataChanged()
        {
            var nodes = Curve.VirtualControlPoints;

            var nodeCount = Math.Max(0, nodes.Count - 2); //ignore first and last knot (which do not modify correct bernstein polygon)

            if (nodeCount * 3 != _curveVertexData.Length)
            {
                _curveIndices = new uint[Math.Max(0, (4 * nodeCount - 4) / 3)];
                _curveVertexData = new float[3 * nodeCount];
            }

            for(int i = 0; i < _curveIndices.Length; i++)
            {
                _curveIndices[i] = (uint)(i - (i / 4));
            }

            for(int i = 0; i < nodeCount; i++)
            {
                SetVertex(_curveVertexData, nodes[i+1], i);
            }
        }

        private void SourceDataChangedEdges()
        {
            var nodes = Curve.ControlPoints;
            if (nodes.Count * 3 != _edgeVertexData.Length)
            {
                _edgeIndices = new uint[Math.Max(0, nodes.Count * 2 - 2)];
                _edgeVertexData = new float[3 * nodes.Count];
            }


            var offset = _curveVertexData.Length / 3;
            for (uint i = 0; i < _edgeIndices.Length; i++) _edgeIndices[i] = (uint)(offset + ((i + 1) / 2));

            for (int i = 0; i < nodes.Count; i++)
            {
                SetVertex(_edgeVertexData, nodes[i].Transform.WorldPosition, i);
            }

        }

        private void SourceDataChangedVirtualEdges()
        {
            if (Curve.HasVirtualControlPoints)
            {
                var nodes = Curve.VirtualControlPoints;
                if (nodes.Count * 3 != _virtualEdgeVertexData.Length)
                {
                    _virtualEdgeIndices = new uint[Math.Max(0, nodes.Count * 2 - 2)];
                    _virtualEdgeVertexData = new float[3 * nodes.Count];
                }


                var offset = (_curveVertexData.Length / 3) + (_edgeVertexData.Length / 3);
                for (uint i = 0; i < _virtualEdgeIndices.Length; i++) _virtualEdgeIndices[i] = (uint)(offset + ((i + 1) / 2));

                for (int i = 0; i < nodes.Count; i++)
                {
                    SetVertex(_virtualEdgeVertexData, nodes[i], i);
                }
            }
        }

        private void CheckCombineArrays()
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
        #endregion

        #region VirtualNodes

        private List<INode> _virtualNodes = new List<INode>();

        private void UpdateVirtualPoints()
        {
            PauseTransformHandling();
            if (ShowVirtualControlPoints)
            {
                UpdateVirtualPointsCount();
                for (int i = 0; i < _virtualNodes.Count; i++)
                {
                    _virtualNodes[i].Transform.Position = Curve.VirtualControlPoints[i + 1];
                }
                AttachVirtualPoints();
            }
            else
            {
                DetachVirtualPoints();
            }
            ResumeTransformHandling();
        }

        private void DetachVirtualPoints()
        {
            foreach (var node in _virtualNodes)
            {
                if (node.Transform.Parent != null)
                {
                    node.Transform.Parent.DetachChild(node);
                }
            }
        }

        private void AttachVirtualPoints()
        {
            foreach (var node in _virtualNodes)
            {
                if (node.Transform.Parent == null)
                {
                    Registry.VirtualNodeFactory.DefaultParent.AttachChild(node);
                }
            }
        }

        private void UpdateVirtualPointsCount()
        {
            while (_virtualNodes.Count < Curve.VirtualControlPoints.Count - 2)
            {
                _virtualNodes.Add(Registry.VirtualNodeFactory.CreateVirtualPoint(Vector3.Zero));
            }
            while (_virtualNodes.Count > Curve.VirtualControlPoints.Count - 2)
            {
                if (_virtualNodes.Count == 0) break;
                var node = _virtualNodes[_virtualNodes.Count - 1];
                _virtualNodes.RemoveAt(_virtualNodes.Count - 1);
                node.Dispose();
            }
        }

        private INode _currentVirtualNode;
        private void HandleSelectionChanged()
        {
            if (_currentVirtualNode != null)
            {
                _currentVirtualNode.PropertyChanged -= HandleVirtualTranslation;
            }
            if (_selectionManager.MainSelection is VirtualNode && _virtualNodes.Contains(_selectionManager.MainSelection))
            {
                _currentVirtualNode = _selectionManager.MainSelection;
                _currentVirtualNode.PropertyChanged += HandleVirtualTranslation;
            }
            else
            {
                _currentVirtualNode = null;
            }
        }

        private void HandleVirtualTranslation(object sender, PropertyChangedEventArgs e)
        {
            var nodes = Curve.ControlPoints;
            var idx = _virtualNodes.IndexOf(_currentVirtualNode);
            int toMoveIdx, refIdx;
            Vector3 pos;
            if(idx%3==0) // Connecting Bernstein Node
            {
                toMoveIdx = (idx / 3) + 1;
                refIdx =    (idx / 3) + 2;
                var ref2Idx = (idx / 3);

                var offset = (nodes[refIdx].Transform.WorldPosition - nodes[ref2Idx].Transform.WorldPosition)/6.0f;
                pos = _currentVirtualNode.Transform.WorldPosition + offset;
            }
            else if ((idx-1)%3==0) // Bernstein on spline polyline edge
            {
                toMoveIdx = ((idx - 1) / 3) + 1;
                refIdx =    ((idx - 1) / 3) + 2;
                pos = _currentVirtualNode.Transform.WorldPosition;
            }
            else //if ((idx+1)%3==0) // Bernstein on spline polyline edge
            {
                toMoveIdx = ((idx + 1) / 3) + 1;
                refIdx = ((idx + 1) / 3);
                pos = _currentVirtualNode.Transform.WorldPosition;
            }

            var refPos = nodes[refIdx].Transform.WorldPosition;
            var twoThirds = pos - refPos;
            var finalPos = refPos + 1.5f * twoThirds;
            nodes[toMoveIdx].Transform.WorldPosition = finalPos;
            DataChangedWrapper();
        }

        private void PauseTransformHandling()
        {
            if (_currentVirtualNode != null) _currentVirtualNode.PropertyChanged -= HandleVirtualTranslation;
        }

        private void ResumeTransformHandling()
        {
            if (_currentVirtualNode != null) _currentVirtualNode.PropertyChanged += HandleVirtualTranslation;
        }

        private void DisposeVirtualNodes()
        {
            if (_currentVirtualNode != null)
            {
                _currentVirtualNode.PropertyChanged -= HandleVirtualTranslation;
                _currentVirtualNode = null;
            }
            for(int i = _virtualNodes.Count - 1; i >= 0; i--)
            {
                _virtualNodes[i].Dispose();
            }
            _virtualNodes.Clear();
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
