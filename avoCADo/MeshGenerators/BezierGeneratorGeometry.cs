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
    public class BezierGeneratorGeometry : IMeshGenerator, ICircularDependent<INode>
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


        private List<DrawCall> _drawCalls = new List<DrawCall>(3);
        public IList<DrawCall> DrawCalls 
        { 
            get
            {
                _drawCalls.Clear();
                if (ShowEdges)
                {
                    _drawCalls.Add(new DrawCall(0, _curveIndices.Length, DrawCallShaderType.Curve, RenderConstants.CURVE_SIZE));
                    _drawCalls.Add(new DrawCall(_curveIndices.Length, _edgeIndices.Length, DrawCallShaderType.Default, RenderConstants.POLYGON_SIZE, RenderConstants.POLYGON_DEFAULT_COLOR, RenderConstants.POLYGON_SELECTED_COLOR));
                    _drawCalls.Add(new DrawCall(_curveIndices.Length + _edgeIndices.Length, _virtualEdgeIndices.Length, DrawCallShaderType.Default, RenderConstants.POLYGON_SIZE, RenderConstants.POLYGON_DEFAULT_COLOR, RenderConstants.POLYGON_SELECTED_COLOR));
                }
                else
                {
                    _drawCalls.Add(new DrawCall(0, _curveIndices.Length, DrawCallShaderType.Curve, RenderConstants.CURVE_SIZE));
                }
                return _drawCalls;
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

        public void RefreshDataPreRender()
        {
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
                DrawCalls.Add(new DrawCall(0, _curveIndices.Length, DrawCallShaderType.Curve, RenderConstants.CURVE_SIZE));
                DrawCalls.Add(new DrawCall(_curveIndices.Length, _edgeIndices.Length, DrawCallShaderType.Default, RenderConstants.POLYGON_SIZE));
                DrawCalls.Add(new DrawCall(_curveIndices.Length + _edgeIndices.Length, _virtualEdgeIndices.Length, DrawCallShaderType.Default, RenderConstants.POLYGON_SIZE));
            }
            else if(DrawCalls.Count != 1)
            {
                DrawCalls.Clear();
                DrawCalls.Add(new DrawCall(0, _curveIndices.Length, DrawCallShaderType.Curve, RenderConstants.CURVE_SIZE));
            }
        }

        private void SourceDataChanged()
        {
            var nodes = Curve.BernsteinControlPoints;

            var nodeCount = Math.Max(0, nodes.Count);
            var correctedNodeCount = FixNodeCount(nodeCount); //we correct node count to always send four bernstein knots to geometry shader
            if (correctedNodeCount * 3 != _curveVertexData.Length)
            {
                _curveIndices = new uint[Math.Max(0, (4 * correctedNodeCount - 4) / 3)];
                _curveVertexData = new float[3 * correctedNodeCount];
            }

            for(int i = 0; i < _curveIndices.Length; i++)
            {
                _curveIndices[i] = (uint)(i - (i / 4));
            }

            for(int i = 0; i < nodeCount; i++) // fill data only from existing knots
            {
                VBOUtility.SetVertex(_curveVertexData, nodes[i], i);
            }

            for(int i = nodeCount; i < correctedNodeCount; i++) //fill the rest with nan values
            {
                VBOUtility.SetVertex(_curveVertexData, new Vector3(float.NaN, float.NaN, float.NaN), i);
            }
        }

        /// <summary>
        /// Modifies node count to always have full bernstein polygons (for geometry shader)
        /// </summary>
        /// <param name="nodeCount"></param>
        /// <returns></returns>
        private int FixNodeCount(int nodeCount)
        {
            if (nodeCount == 0) return nodeCount;
            if(nodeCount < 4 && nodeCount > 0)
            {
                return 4;
            }
            else
            {
                var ret = (nodeCount - 4) % 3;
                if (ret == 0) return nodeCount;
                else return nodeCount + 3 - ret;
            }
        }

        private void SourceDataChangedEdges()
        {
            var nodes = Curve.PolygonPoints;
            if (nodes.Count * 3 != _edgeVertexData.Length)
            {
                _edgeIndices = new uint[Math.Max(0, nodes.Count * 2 - 2)];
                _edgeVertexData = new float[3 * nodes.Count];
            }


            var offset = _curveVertexData.Length / 3;
            for (uint i = 0; i < _edgeIndices.Length; i++) _edgeIndices[i] = (uint)(offset + ((i + 1) / 2));

            for (int i = 0; i < nodes.Count; i++)
            {
                VBOUtility.SetVertex(_edgeVertexData, nodes[i], i);
            }

        }

        private void SourceDataChangedVirtualEdges()
        {
            if (Curve is IVirtualControlPoints)
            {
                var vCtrlPoints = Curve as IVirtualControlPoints;
                var nodes = vCtrlPoints.VirtualControlPoints;
                if (nodes.Count * 3 != _virtualEdgeVertexData.Length)
                {
                    _virtualEdgeIndices = new uint[Math.Max(0, nodes.Count * 2 - 2)];
                    _virtualEdgeVertexData = new float[3 * nodes.Count];
                }


                var offset = (_curveVertexData.Length / 3) + (_edgeVertexData.Length / 3);
                for (uint i = 0; i < _virtualEdgeIndices.Length; i++) _virtualEdgeIndices[i] = (uint)(offset + ((i + 1) / 2));

                for (int i = 0; i < nodes.Count; i++)
                {
                    VBOUtility.SetVertex(_virtualEdgeVertexData, nodes[i], i);
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
            var vCtrlPoints = Curve as IVirtualControlPoints;
            if (vCtrlPoints != null)
            {
                PauseTransformHandling();
                if (ShowVirtualControlPoints)
                {
                    UpdateVirtualPointsCount();
                    for (int i = 0; i < _virtualNodes.Count; i++)
                    {
                        _virtualNodes[i].Transform.Position = vCtrlPoints.VirtualControlPoints[i + 1];
                    }
                    AttachVirtualPoints();
                }
                else
                {
                    DetachVirtualPoints();
                }
                ResumeTransformHandling();
            }
        }

        private void DetachVirtualPoints()
        {
            foreach (var node in _virtualNodes)
            {
                if (node.Transform.ParentNode != null)
                {
                    node.Transform.ParentNode.DetachChild(node);
                }
            }
        }

        private void AttachVirtualPoints()
        {
            foreach (var node in _virtualNodes)
            {
                if (node.Transform.ParentNode == null)
                {
                    Registry.VirtualNodeFactory.DefaultParent.AttachChild(node);
                }
            }
        }

        private void UpdateVirtualPointsCount()
        {
            var vCtrlPoints = Curve as IVirtualControlPoints;
            if (vCtrlPoints != null)
            {
                while (_virtualNodes.Count < vCtrlPoints.VirtualControlPoints.Count - 2)
                {
                    _virtualNodes.Add(Registry.VirtualNodeFactory.CreateVirtualPoint(Vector3.Zero));
                }
                while (_virtualNodes.Count > vCtrlPoints.VirtualControlPoints.Count - 2)
                {
                    if (_virtualNodes.Count == 0) break;
                    var node = _virtualNodes[_virtualNodes.Count - 1];
                    _virtualNodes.RemoveAt(_virtualNodes.Count - 1);
                    node.Dispose();
                }
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
    }
}
