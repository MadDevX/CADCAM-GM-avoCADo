using avoCADo.Rendering.Renderers;
using avoCADo.Utility;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace avoCADo.CNC
{
    public enum ToolType
    {
        Round = 0,
        Flat = 1
    }

    public struct CNCTool
    {
        public readonly ToolType Type;
        public readonly float Radius;
        public readonly float RadiusSqr;

        public CNCTool(ToolType type, float radius)
        {
            Type = type;
            Radius = radius;
            RadiusSqr = Radius * Radius;
        }
    }

    public class MaterialBlock : IDisposable
    {
        //        _______________
        // height|               |
        //       |               |
        //       |               |
        //      0|_______________|>width
        //        0
        public float[] HeightMap { get; private set; }
        public float DefaultHeightValue { get; set; }

        private int _width;
        private int _height;

        public int Height 
        {
            get => _height;
            set
            {
                _height = value;
                UpdateCoordMappings();
                UpdateMesh();
                ResetHeightMap();
                TextureManager.ResetTexture(Width, Height);
            }
        }
        public int Width 
        {
            get => _width;
            set
            {
                _width = value;
                UpdateCoordMappings();
                UpdateMesh();
                ResetHeightMap();
                TextureManager.ResetTexture(Width, Height);
            }
        }

        public float MinHeightValue { get; set; }

        private float _worldWidth;
        private float _worldHeight;

        public float WorldWidth 
        { 
            get => _worldWidth; 
            set
            {
                _worldWidth = value;
                UpdateCoordMappings();
                UpdateMesh();
            }
        }

        public float WorldHeight 
        {
            get => _worldHeight;
            set
            {
                _worldHeight = value;
                UpdateCoordMappings();
                UpdateMesh();
            }
        }

        //Speedup operations
        private float _worldWidthToIndex;
        private float _worldHeightToIndex;

        private float _indexWidthToWorld;
        private float _indexHeightToWorld;

        private Vector2 _offsetVector;

        private Vector2 _blockCenter = Vector2.Zero;

        private Func<float, float, CNCTool, float>[] _funcList = new Func<float, float, CNCTool, float>[2];

        public MaterialBlockTextureManager TextureManager { get; }
        private MeshRenderer _renderer;

        public MaterialBlock(int width, int height, float worldWidth, float worldHeight, float defaultHeightValue, float minHeightValue, MeshRenderer renderer)
        {
            _funcList[0] = HeightByDistFromCenterRound;
            _funcList[1] = HeightByDistFromCenterFlat;
            //X Y resolution of height texture
            _renderer = renderer;
            TextureManager = new MaterialBlockTextureManager(width, height);
            MinHeightValue = minHeightValue;
            DefaultHeightValue = defaultHeightValue;
            Width = width;
            Height = height;


            //Scaling used to interpret block/heightmap in world space coordinates
            WorldWidth = worldWidth;
            WorldHeight = worldHeight;

            //Map coords from [(0.0 0.0),(worldWidth, worldHeight)] to [(0.0 0.0),(width, height)]
            UpdateCoordMappings();

            //Used to transform worldPosition to localCoordinates (0.0, 0.0) : (_worldWidth, _worldHeight)
            _offsetVector = new Vector2(WorldWidth * 0.5f, WorldHeight * 0.5f);

            _renderer.TextureProvider = TextureManager;
            UpdateMesh();
            UpdateTexture();
        }

        private void UpdateCoordMappings()
        {
            _worldWidthToIndex = (1.0f / WorldWidth) * Width;
            _worldHeightToIndex = (1.0f / WorldHeight) * Height;

            _indexWidthToWorld = (1.0f / (Width - 1)) * WorldWidth;
            _indexHeightToWorld = (1.0f / (Height - 1)) * WorldHeight;
        }

        public void Dispose()
        {
            TextureManager.Dispose();
        }

        public void ResetHeightMap()
        {
            HeightMap = new float[Width * Height];
            ResetHeightMapValues();
        }

        public void ResetHeightMapValues()
        {
            for (int i = 0; i < Width * Height; i++) HeightMap[i] = DefaultHeightValue;
        }

        public void UpdateTexture()
        {
            TextureManager.UpdateTexture(Width, Height, HeightMap);
        }

        private void UpdateMesh()
        {
            if (Width > 1 && Height > 1)
            {
                _renderer.SetMesh(MeshUtility.CreatePlaneMesh(Width, Height, WorldWidth, WorldHeight));
            }
        }

        private int GetIndex(int x, int z)
        {
            return x + z * Width;
        }

        private void SetPixel(int x, int z, float value)
        {
            if (x < Width && z < Height)
            {
                HeightMap[GetIndex(x, z)] = value;
            }
        }

        private void DrillPixel(int x, int z, float value, Vector3 moveDirection, ToolType type)
        {
            if (x >= 0 && x < Width && z >= 0 && z < Height)
            {
                var idx = GetIndex(x, z);
                if(type == ToolType.Flat && moveDirection.Y < 0.0f && value < HeightMap[idx])
                    throw new InvalidOperationException("Flat tool drills vertically into the material!");
                if (value < MinHeightValue) 
                    throw new InvalidOperationException("Tool drills into base of the material!");
                HeightMap[idx] = Math.Min(HeightMap[idx], value);
            }
        }

        private bool CheckContactPixel(int x, int z, float value)
        {
            if (x >= 0 && x < Width && z >= 0 && z < Height)
            {
                var idx = GetIndex(x, z);
                return HeightMap[idx] > value; //TODO: maybe >=
            }
            return false;
        }

        private (int x, int z) GetCoordsFromPosition(Vector2 worldPosition)
        {
            var localPosition = worldPosition - _blockCenter;
            var offsetPosition = localPosition + _offsetVector;

            var x = (int)(offsetPosition.X * _worldWidthToIndex);
            var y = (int)(offsetPosition.Y * _worldHeightToIndex);

            return (x, y);
        }

        private Vector2 IdxToWorld(int x, int z)
        {
            var wX = x * _indexWidthToWorld;
            var wZ = z * _indexHeightToWorld;
            return new Vector2(wX, wZ) - _offsetVector;
        }

        public void DrillCircleAtPosition(Vector3 toolPosition, Vector3 moveDirection, CNCTool tool)
        {
            var radius = tool.Radius;
            var radiusSqr = tool.RadiusSqr;
            var luPos = new Vector2(toolPosition.X - radius, toolPosition.Z - radius);
            var rbPos = new Vector2(toolPosition.X + radius, toolPosition.Z + radius);

            var lu = GetCoordsFromPosition(luPos);
            var rb = GetCoordsFromPosition(rbPos);
            int toolTypeIdx = (int)tool.Type;

            Parallel.For(lu.z, rb.z,
                (y) =>
                {
                    for (int x = lu.x; x < rb.x; x++)
                    {
                        var pos = IdxToWorld(x, y);
                        var offX = toolPosition.X - pos.X;
                        var offY = toolPosition.Z - pos.Y;
                        var distSqr = offX * offX + offY * offY;
                        if (distSqr <= radiusSqr)
                        {
                            DrillPixel(x, y, _funcList[toolTypeIdx](toolPosition.Y, distSqr, tool), moveDirection, tool.Type);
                        }
                    }
                }
            );
            
        }

        //private List<Point> _pointBuffer = new List<Point>(1000);
        private List<Vector3> _positionsBuffer = new List<Vector3>(1000);

        public void DrillCircleAtSegment(Vector3 start, Vector3 end, CNCTool tool)
        {
            //_pointBuffer.Clear(); //TODO: first aggregate all points, then simulate
            _positionsBuffer.Clear();
            var startXZ = start.Xz;
            var endXZ = end.Xz;
            var diffX = Math.Abs(start.X - end.X);
            var diffZ = Math.Abs(start.Z - end.Z);
            var dir = end - start;

            var startIdx = GetCoordsFromPosition(startXZ);
            var endIdx = GetCoordsFromPosition(endXZ);

            //Slower but more accurate
            var steps = Math.Max(Math.Abs(startIdx.x - endIdx.x), Math.Abs(startIdx.z - endIdx.z)) + 1;
            for(int i = 0; i < steps; i++)
            {
                var interpolated = Vector3.Lerp(start, end, i / (float)(steps - 1));
                _positionsBuffer.Add(interpolated);
            }
            foreach(var p in _positionsBuffer)
            {
                DrillCircleAtPosition(p, dir, tool);
            }

            //Faster but much uglier
            //bool hor = diffX > diffZ; //get bigger difference for numerical stability
            //Algorithms.Bresenham(startIdx.x, startIdx.z, endIdx.x, endIdx.z, _pointBuffer);
            //foreach (var p in _pointBuffer)
            //{
            //    var pos = IdxToWorld(p.X, p.Y);
            //    var t = hor ? (pos.X - startXZ.X) / (endXZ.X - startXZ.X) : (pos.Y - startXZ.Y) / (endXZ.Y - startXZ.Y);
            //    var y = start.Y + MathHelper.Clamp(t, 0.0f, 1.0f) * (end.Y - start.Y);
            //    DrillCircleAtPosition(new Vector3(pos.X, y, pos.Y), tool);
            //}

        }

        private bool InContact(Vector3 toolPosition, CNCTool tool)
        {
            var radius = tool.Radius;
            var radiusSqr = tool.RadiusSqr;
            var luPos = new Vector2(toolPosition.X - radius, toolPosition.Z - radius);
            var rbPos = new Vector2(toolPosition.X + radius, toolPosition.Z + radius);

            var lu = GetCoordsFromPosition(luPos);
            var rb = GetCoordsFromPosition(rbPos);
            int toolTypeIdx = (int)tool.Type;

            for (int y = lu.z; y < rb.z; y++)
            {
                for (int x = lu.x; x < rb.x; x++)
                {
                    var pos = IdxToWorld(x, y);
                    var offX = toolPosition.X - pos.X;
                    var offY = toolPosition.Z - pos.Y;
                    var distSqr = offX * offX + offY * offY;
                    if (distSqr <= radiusSqr)
                    {
                        if(CheckContactPixel(x, y, _funcList[toolTypeIdx](toolPosition.Y, distSqr, tool)))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private float HeightByDistFromCenterRound(float toolY, float distanceSqr, CNCTool tool)
        {
            var remaining = tool.RadiusSqr - distanceSqr;
            return toolY - (float)Math.Sqrt(remaining) + tool.Radius;
        }

        private float HeightByDistFromCenterFlat(float toolY, float distanceSqr, CNCTool tool)
        {
            return toolY;
        }
    }
}
