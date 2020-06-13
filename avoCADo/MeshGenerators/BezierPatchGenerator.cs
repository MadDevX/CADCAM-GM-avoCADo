using avoCADo.Constants;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo
{
    public enum WrapMode
    {
        None,
        Column,
        Row
    }

    public class BezierPatchGenerator : IMeshGenerator, ICircularDependent<INode>
    {
        protected virtual DrawCallShaderType SurfaceDrawType { get; } = DrawCallShaderType.SurfaceBezier;
        protected virtual int PatchCount { get; } = RenderConstants.PATCH_COUNT;
        private List<DrawCall> _drawCalls = new List<DrawCall>(3);
        public IList<DrawCall> DrawCalls
        {
            get
            {
                _drawCalls.Clear();
                if(ShowEdges)
                {

                    _drawCalls.Add(new DrawCall(0, _surfaceIndices.Length/2, SurfaceDrawType, RenderConstants.SURFACE_SIZE, PatchCount, IsolineDivisionsU, 64));
                    _drawCalls.Add(new DrawCall(_surfaceIndices.Length/2, _surfaceIndices.Length/2, SurfaceDrawType, RenderConstants.SURFACE_SIZE, PatchCount, IsolineDivisionsV, 64));
                    _drawCalls.Add(new DrawCall(_surfaceIndices.Length, _edgeIndices.Length, DrawCallShaderType.Default, RenderConstants.POLYGON_SIZE, RenderConstants.POLYGON_DEFAULT_COLOR, RenderConstants.POLYGON_SELECTED_COLOR));
                }
                else
                {
                    _drawCalls.Add(new DrawCall(0, _surfaceIndices.Length / 2, SurfaceDrawType, RenderConstants.SURFACE_SIZE, PatchCount, IsolineDivisionsU, 64));
                    _drawCalls.Add(new DrawCall(_surfaceIndices.Length / 2, _surfaceIndices.Length / 2, SurfaceDrawType, RenderConstants.SURFACE_SIZE, PatchCount, IsolineDivisionsV, 64));
                }
                return _drawCalls;
            }
        }
        public event Action OnParametersChanged;

        public int IsolineDivisionsU { get; set; } = 4;
        public int IsolineDivisionsV { get; set; } = 4;
        public bool ShowEdges { get; set; } = false;

        private bool _showControlPoints = true; 
        public bool ShowControlPoints 
        {
            get => _showControlPoints; 
            set
            {
                _showControlPoints = value;
                foreach(var node in _node.Children)
                {
                    node.IsSelectable = value;
                }
            }
        }


        public int HorizontalPatches
        { 
            get
            {
                return Surface.USegments;
            }
            set
            {
                _ctrlPointManager.UpdateControlPoints(_defaultPosition, value, VerticalPatches);
            }
        }

        public int VerticalPatches
        {
            get
            {
                return Surface.VSegments;
            }
            set
            {
                _ctrlPointManager.UpdateControlPoints(_defaultPosition, HorizontalPatches, value);
            }
        }

        public float SurfaceWidthOrRadius { get; set; } = 1.0f;
        public float SurfaceHeight { get; set; } = 1.0f;
        public IBezierSurface Surface { get; }

        public WrapMode WrapMode 
        { 
            get
            {
                return _wrapMode;
            }
            set
            {
                _wrapMode = value;
                _ctrlPointManager.UpdateControlPoints(_defaultPosition, HorizontalPatches, VerticalPatches);
            }
        }

        private WrapMode _wrapMode = WrapMode.None;
        private INode _node;
        private NodeFactory _nodeFactory;
        private BezierC0PatchControlPointManager _ctrlPointManager;

        private int _defaultHorizontalPatches, _defaultVerticalPatches;
        private Vector3 _defaultPosition;
        private CoordList<INode> _existingNodes = null;

        public BezierPatchGenerator(IBezierSurface surface, NodeFactory nodeFactory, WrapMode patchType, Vector3 position, int horizontalPatches = 1, int verticalPatches = 1, float width = 1.0f, float height = 1.0f)
        {
            Surface = surface;
            _nodeFactory = nodeFactory;
            _wrapMode = patchType;
            _defaultHorizontalPatches = horizontalPatches;
            _defaultVerticalPatches = verticalPatches;
            _defaultPosition = position;
            SurfaceWidthOrRadius = width;
            SurfaceHeight = height;
        }

        public BezierPatchGenerator(IBezierSurface surface, NodeFactory nodeFactory, WrapMode patchType, Vector3 position, int horizontalPatches, int verticalPatches, CoordList<INode> existingNodes)
        {
            Surface = surface;
            _nodeFactory = nodeFactory;
            _wrapMode = patchType;
            _defaultHorizontalPatches = horizontalPatches;
            _defaultVerticalPatches = verticalPatches;
            _defaultPosition = position;
            SurfaceWidthOrRadius = 0.0f;
            SurfaceHeight = 0.0f;
            _existingNodes = existingNodes;
        }

        public void Initialize(INode node)
        {
            _node = node;
            _ctrlPointManager = CreateCPManager(_nodeFactory, this, _node);
            Initialize(_defaultPosition, _defaultHorizontalPatches, _defaultVerticalPatches, _existingNodes);
        }

        protected virtual BezierC0PatchControlPointManager CreateCPManager(NodeFactory nodeFactory, BezierPatchGenerator generator, INode node)
        {
            return new BezierC0PatchControlPointManager(nodeFactory, generator, node);
        }

        private void Initialize(Vector3 position, int horizontalPatches, int verticalPatches, CoordList<INode> existingNodes)
        {
            if (existingNodes == null)
            {
                _ctrlPointManager.UpdateControlPoints(position, horizontalPatches, verticalPatches);
            }
            else
            {
                _ctrlPointManager.UpdateControlPoints(horizontalPatches, verticalPatches, existingNodes);
            }
        }


        public void Dispose()
        {
            _ctrlPointManager.Dispose();
        }


        private uint[] _surfaceIndices = new uint[0];
        private float[] _vertices = new float[0];
        private uint[] _edgeIndices = new uint[0];
        private uint[] _indices = new uint[0];
        public uint[] GetIndices()
        {
            return _indices;
        }

        public float[] GetVertices()
        {
            return _vertices;
        }

        private void UpdateBufferDataWrapper()
        {
            UpdateBufferData();
            UpdateEdgesBuffer();
            CheckCombineArrays();
            OnParametersChanged?.Invoke();
        }

        private void UpdateEdgesBuffer()
        {
            var cps = Surface.ControlPoints;
            var edgeIndexCount = 2 * (2 * (cps.Width - 1) * (cps.Height - 1) + (cps.Width - 1) + (cps.Height - 1)); //two edges per vertex, except last ones - last vertices have 1 outcoming edge
            if (_edgeIndices.Length != edgeIndexCount)
            {
                Array.Resize(ref _edgeIndices, edgeIndexCount);
            }
            var _indicesIdx = 0;
            for(int u = 0; u < cps.Width; u++)
            {
                for(int v = 0; v < cps.Height; v++)
                {
                    if (u < cps.Width - 1)
                    {
                        _edgeIndices[_indicesIdx] = (uint)(u + v * cps.Width);
                        _indicesIdx++;
                        _edgeIndices[_indicesIdx] = (uint)((u + 1) + v * cps.Width);
                        _indicesIdx++;
                    }

                    if (v < cps.Height - 1)
                    {
                        _edgeIndices[_indicesIdx] = (uint)(u + v * cps.Width);
                        _indicesIdx++;
                        _edgeIndices[_indicesIdx] = (uint)(u + (v + 1) * cps.Width);
                        _indicesIdx++;
                    }
                }
            }
        }

        private void UpdateBufferData()
        {
            var cps = Surface.ControlPoints;
            if(_vertices.Length != cps.Count * 3)
            {
                Array.Resize(ref _vertices, cps.Count * 3);
                Array.Resize(ref _surfaceIndices, Surface.USegments * Surface.VSegments * 16 * 2); //isolines in two directions, thus *2
            }

            for(int i = 0; i < cps.Width; i++)
            {
                for(int j = 0; j < cps.Height; j++)
                {
                    VBOUtility.SetVertex(_vertices, cps[i, j].Transform.WorldPosition, i + j * cps.Width);
                }
            }

            SetPatchIndices(_surfaceIndices);
        }

        private void SetPatchIndices(uint[] surfaceIndices)
        {
            var cps = Surface.ControlPoints;
            int indicesIdx = 0; //should be dependent on currently processed patch
            for (int vPatch = 0; vPatch < Surface.VSegments; vPatch++)
            {
                for (int uPatch = 0; uPatch < Surface.USegments; uPatch++)
                {
                    var uIdx = GetPatchOffset(uPatch);
                    var vIdx = GetPatchOffset(vPatch);
                    for (int j = vIdx; j < vIdx + 4; j++)
                    {
                        for (int i = uIdx; i < uIdx + 4; i++)
                        {
                            surfaceIndices[indicesIdx] = (uint)(i + j * cps.Width);
                            indicesIdx++;
                        }
                    }

                }
            }

            for (int vPatch = 0; vPatch < Surface.VSegments; vPatch++)
            {
                for (int uPatch = 0; uPatch < Surface.USegments; uPatch++)
                {
                    var uIdx = GetPatchOffset(uPatch);
                    var vIdx = GetPatchOffset(vPatch);
                    for (int i = uIdx; i < uIdx + 4; i++)
                    {
                        for (int j = vIdx; j < vIdx + 4; j++)
                        {
                            surfaceIndices[indicesIdx] = (uint)(i + j * cps.Width);
                            indicesIdx++;
                        }
                    }

                }
            }
        }

        protected virtual int GetPatchOffset(int patch)
        {
            return patch * 3;
        }

        private void CheckCombineArrays()
        {
            if (_indices.Length != _surfaceIndices.Length + _edgeIndices.Length)
            {
                _indices = new uint[_surfaceIndices.Length + _edgeIndices.Length];
            }
            Array.Copy(_surfaceIndices, 0, _indices, 0, _surfaceIndices.Length);
            Array.Copy(_edgeIndices, 0, _indices, _surfaceIndices.Length, _edgeIndices.Length);
        }

        public void RefreshDataPreRender()
        {
            if (_ctrlPointManager.ShouldUpdateData)
            {
                UpdateBufferDataWrapper();
                
                _ctrlPointManager.ShouldUpdateData = false;
            }
        }
    }
}
