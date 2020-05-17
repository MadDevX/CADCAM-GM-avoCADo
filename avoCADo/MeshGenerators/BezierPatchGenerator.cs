using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo
{
    public enum PatchType
    {
        Flat,
        Cylinder
    }

    public class BezierPatchGenerator : IMeshGenerator, IDependent<INode>
    {
        public IList<DrawCall> DrawCalls
        {
            get
            {
                if(ShowEdges)
                {
                    return new List<DrawCall>()
                    {
                        new DrawCall(0, _surfaceIndices.Length, DrawCallShaderType.Surface, RenderConstants.SURFACE_SIZE, IsolineDivisions, 64),
                        new DrawCall(_surfaceIndices.Length, _edgeIndices.Length, DrawCallShaderType.Default, RenderConstants.POLYGON_SIZE, RenderConstants.POLYGON_COLOR)
                    };
                }
                else
                {
                    return new List<DrawCall>() { new DrawCall(0, _surfaceIndices.Length, DrawCallShaderType.Surface, RenderConstants.SURFACE_SIZE, IsolineDivisions, 64) };
                }
            }
        }
        public event Action OnParametersChanged;

        public int IsolineDivisions { get; set; } = 4;
        public bool ShowEdges { get; set; } = false;

        public int HorizontalPatches
        { 
            get
            {
                return Surface.USegments;
            }
            set
            {
                _ctrlPointManager.UpdateControlPoints(value, VerticalPatches);
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
                _ctrlPointManager.UpdateControlPoints(HorizontalPatches, value);
            }
        }

        public float SurfaceWidthOrRadius { get; set; } = 1.0f;
        public float SurfaceHeight { get; set; } = 1.0f;
        public IBezierSurface Surface { get; }

        public PatchType PatchType 
        { 
            get
            {
                return _patchType;
            }
            set
            {
                _patchType = value;
                _ctrlPointManager.UpdateControlPoints(HorizontalPatches, VerticalPatches);
            }
        }

        private PatchType _patchType = PatchType.Flat;
        private INode _parentNode;
        private NodeFactory _nodeFactory;
        private BezierC0PatchControlPointManager _ctrlPointManager;

        private int _defaultHorizontalPatches, _defaultVerticalPatches;

        public BezierPatchGenerator(IBezierSurface surface, NodeFactory nodeFactory, PatchType patchType, int horizontalPatches = 1, int verticalPatches = 1, float width = 1.0f, float height = 1.0f)
        {
            Surface = surface;
            _nodeFactory = nodeFactory;
            _patchType = patchType;
            _defaultHorizontalPatches = horizontalPatches;
            _defaultVerticalPatches = verticalPatches;
            SurfaceWidthOrRadius = width;
            SurfaceHeight = height;
        }

        public void Initialize(INode node)
        {
            _parentNode = node;
            _ctrlPointManager = new BezierC0PatchControlPointManager(_nodeFactory, this, _parentNode);
            Initialize(_defaultHorizontalPatches, _defaultVerticalPatches);
        }

        private void Initialize(int horizontalPatches, int verticalPatches)
        {
            _ctrlPointManager.UpdateControlPoints(horizontalPatches, verticalPatches);
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

            int indicesIdx = 0; //should be dependent on currently processed patch
            for(int vPatch = 0; vPatch < Surface.VSegments; vPatch++)
            {
                for(int uPatch = 0; uPatch < Surface.USegments; uPatch++)
                {
                    var uIdx = uPatch * 3;
                    var vIdx = vPatch * 3;
                    for(int j = vIdx; j < vIdx + 4; j++)
                    {
                        for(int i = uIdx; i < uIdx + 4; i++)
                        {
                            _surfaceIndices[indicesIdx] = (uint)(i + j * cps.Width);
                            indicesIdx++;
                        }
                    }

                }
            }
            for (int vPatch = 0; vPatch < Surface.VSegments; vPatch++)
            {
                for (int uPatch = 0; uPatch < Surface.USegments; uPatch++)
                {
                    var uIdx = uPatch * 3;
                    var vIdx = vPatch * 3;
                    for (int i = uIdx; i < uIdx + 4; i++)
                    {
                        for (int j = vIdx; j < vIdx + 4; j++)
                        {
                            _surfaceIndices[indicesIdx] = (uint)(i + j * cps.Width);
                            indicesIdx++;
                        }
                    }

                }
            }
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
