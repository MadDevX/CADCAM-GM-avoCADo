using avoCADo.Constants;
using avoCADo.Miscellaneous;
using avoCADo.ParametricObjects.Curves;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo.MeshGenerators
{
    public class GregoryPatchGenerator : IMeshGenerator
    {
        private List<DrawCall> _drawCalls = new List<DrawCall>(3);
        protected virtual int PatchCount { get; } = RenderConstants.GREGORY_PATCH_COUNT;
        public IList<DrawCall> DrawCalls
        {
            get
            {
                _drawCalls.Clear();
                _drawCalls.Add(new DrawCall(0, _indices.Length, DrawCallShaderType.SurfaceGregory, RenderConstants.SURFACE_SIZE, PatchCount, IsolineDivisionsU, 64));
                return _drawCalls;
            }
        }
        public int IsolineDivisionsU { get; set; } = 4;
        public int IsolineDivisionsV { get; set; } = 4;
        public bool ShowEdges { get; set; } = false;

        public event Action OnParametersChanged;
        private IList<CoordList<INode>> _boundaryCoords;

        private IList<Vector3[]> _cps = new List<Vector3[]> { new Vector3[20], new Vector3[20], new Vector3[20] };

        private uint[] _indices = new uint[0];
        private float[] _vertices = new float[0];

        private bool _shouldUpdateData = false;

        private INode _a, _b, _c;

        public GregoryPatchGenerator(INode a, INode b, INode c)
        {
            _a = a;
            _b = b;
            _c = c;

            var surfA = (a.Renderer.GetGenerator() as BezierPatchGenerator).Surface;
            var surfB = (b.Renderer.GetGenerator() as BezierPatchGenerator).Surface;
            var surfC = (c.Renderer.GetGenerator() as BezierPatchGenerator).Surface;
            _boundaryCoords = LoopDetector.GetLoopedCoords(surfA, surfB, surfC);
            UpdateControlPoints();

            Initialize();
        }

        private void Initialize()
        {
            _a.PropertyChanged += DataChanged;
            _b.PropertyChanged += DataChanged;
            _c.PropertyChanged += DataChanged;
        }

        public void Dispose()
        {
            _a.PropertyChanged -= DataChanged;
            _b.PropertyChanged -= DataChanged;
            _c.PropertyChanged -= DataChanged;
        }

        private void UpdateControlPoints()
        {
            SetPositions();
            UpdateBuffers();
        }
        
        private void UpdateBuffers()
        {
            if (_vertices == null || _vertices.Length != 20 * 3 * 3)
            {
                Array.Resize(ref _vertices, 20 * 3 * 3);
                Array.Resize(ref _indices, 20 * 3);
            }
            UpdateVertices();
            UpdateIndices();
        }

        private void UpdateVertices()
        {
            for(int i = 0; i < 3; i++)
            {
                for(int j = 0; j < 20; j++)
                {
                    VBOUtility.SetVertex(_vertices, _cps[i][j], j + i * 20);
                }
            }
        }

        private void UpdateIndices()
        {
            for (int i = 0; i < _indices.Length; i++) _indices[i] = (uint)i;
        }

        private void SetPositions()
        {
            var boundary = SplitBoundaryCurves(0);
            var secondaryBoundary = SplitBoundaryCurves(1);

            for (int i = 0; i < 3; i++)
            {
                _cps[i][0] = boundary[i][3];
                _cps[i][4] = boundary[i][4];
                _cps[i][10] = boundary[i][5];
                _cps[i][16] = boundary[i][6];
                _cps[i][17] = boundary[(i + 1) % 3][1];
                _cps[i][18] = boundary[(i + 1) % 3][2];
                _cps[i][19] = boundary[(i + 1) % 3][3];

                _cps[i][5] = _cps[i][6] = 2.0f * _cps[i][4] - secondaryBoundary[i][4];
                _cps[i][11] = 2.0f * _cps[i][10] - secondaryBoundary[i][5];
                _cps[i][12] = 2.0f * _cps[i][17] - secondaryBoundary[(i + 1) % 3][1];
                _cps[i][13] = _cps[i][14] = 2.0f * _cps[i][18] - secondaryBoundary[(i + 1) % 3][2];
            }
            for (int i = 0; i < 3; i++)
            {
                _cps[(i + 1) % 3][1] = _cps[i][15] = Vector3.Lerp(_cps[i][13], _cps[(i + 1) % 3][5], 0.5f);
            }

            var q = new Vector3[3];
            for (int i = 0; i < 3; i++)
            {
                q[i] = (3.0f * _cps[i][1] - _cps[i][0]) * 0.5f;
            }

            var mid = Vector3.Zero;
            for (int i = 0; i < 3; i++) mid += q[i];
            mid /= 3.0f;

            for (int i = 0; i < 3; i++)
            {
                _cps[i][3] = mid;
                _cps[i][9] = _cps[(i + 1) % 3][2] = (2.0f * q[(i + 1) % 3] + mid) / 3.0f;
            }

            for (int i = 0; i < 3; i++)
            {
                var g2 = (_cps[(i + 1) % 3][4] - _cps[(i + 1) % 3][0]);
                var g0 = ((_cps[(i + 1) % 3][9] - _cps[(i + 1) % 3][3]) + (_cps[i][3] - _cps[i][2])) * 0.5f;
                var g1 = (g0 + g2) * 0.5f;
                var diff = BezierHelper.Bezier2(g0, g1, g2, 1.0f / 3.0f);
                _cps[i][8] = _cps[i][9] - diff;
                _cps[(i + 1) % 3][7] = _cps[(i + 1) % 3][2] + diff;
            }
        }

        private List<List<Vector3>> SplitBoundaryCurves(int depth = 0)
        {
            var splits = new List<List<Vector3>>();
            for (int i = 0; i < 3; i++)
            {
                var split = BezierHelper.SplitBezier(_boundaryCoords[i][0, depth].Transform.WorldPosition,
                                                     _boundaryCoords[i][1, depth].Transform.WorldPosition,
                                                     _boundaryCoords[i][2, depth].Transform.WorldPosition,
                                                     _boundaryCoords[i][3, depth].Transform.WorldPosition, 0.5f);
                splits.Add(split);
            }
            return splits;
        }

        public uint[] GetIndices()
        {
            return _indices;
        }

        public float[] GetVertices()
        {
            return _vertices;
        }

        private void DataChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            _shouldUpdateData = true;
        }

        public void RefreshDataPreRender()
        {
            if(_shouldUpdateData)
            {
                UpdateControlPoints();
            }
        }
    }
}
