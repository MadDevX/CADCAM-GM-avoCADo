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

    public class MaterialBlock
    {
        //        _______________
        // height|               |
        //       |               |
        //       |               |
        //      0|_______________|>width
        //        0
        public float[] HeightMap { get; }
        public int Height { get; }
        public int Width { get; }

        private float _worldWidth;
        private float _worldHeight;

        //Speedup operations
        private float _worldWidthToIndex;
        private float _worldHeightToIndex;

        private float _indexWidthToWorld;
        private float _indexHeightToWorld;

        private Vector2 _offsetVector;

        private Vector2 _blockCenter = Vector2.Zero;

        private Func<float, float, float, float>[] _funcList = new Func<float, float, float, float>[2];

        public MaterialBlock(int width, int height, float worldWidth, float worldHeight, float defaultValue)
        {
            _funcList[0] = HeightByDistFromCenterRound;
            _funcList[1] = HeightByDistFromCenterFlat;
            //X Y resolution of height texture
            Width = width;
            Height = height;

            //Scaling used to interpret block/heightmap in world space coordinates
            _worldWidth = worldWidth;
            _worldHeight = worldHeight;

            //Map coords from [(0.0 0.0),(worldWidth, worldHeight)] to [(0.0 0.0),(width, height)]
            _worldWidthToIndex = (1.0f / _worldWidth) * width;
            _worldHeightToIndex = (1.0f / _worldHeight) * height;

            _indexWidthToWorld = (1.0f / (Width - 1)) * _worldWidth;
            _indexHeightToWorld = (1.0f / (Height - 1)) * _worldHeight;

            //Used to transform worldPosition to localCoordinates (0.0, 0.0) : (_worldWidth, _worldHeight)
            _offsetVector = new Vector2(_worldWidth * 0.5f, _worldHeight * 0.5f);

            HeightMap = new float[width * height];
            for (int i = 0; i < width * height; i++) HeightMap[i] = defaultValue;
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

        private void DrillPixel(int x, int z, float value)
        {
            if (x >= 0 && x < Width && z >= 0 && z < Height)
            {
                var idx = GetIndex(x, z);
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

        public void DrillCircleAtPosition(Vector3 toolPosition, CNCTool tool)
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
                            DrillPixel(x, y, _funcList[toolTypeIdx](toolPosition.Y, distSqr, tool.RadiusSqr));
                        }
                    }
                }
            );
            
        }

        private List<Point> _pointBuffer = new List<Point>(1000);

        public void DrillCircleAtSegment(Vector3 start, Vector3 end, CNCTool tool)
        {
            _pointBuffer.Clear(); //TODO: first aggregate all points, then simulate
            var startXZ = start.Xz;
            var endXZ = end.Xz;
            var diffX = Math.Abs(start.X - end.X);
            var diffZ = Math.Abs(start.Z - end.Z);
            bool straightDown = diffX == 0.0f && diffZ == 0.0f;

            if (straightDown == false)
            {
                bool hor = diffX > diffZ; //get bigger difference for numerical stability
                var startIdx = GetCoordsFromPosition(startXZ);
                var endIdx = GetCoordsFromPosition(endXZ);
                Algorithms.Bresenham(startIdx.x, startIdx.z, endIdx.x, endIdx.z, _pointBuffer);

                foreach (var p in _pointBuffer)
                {
                    var pos = IdxToWorld(p.X, p.Y);
                    var t = hor ? (pos.X - startXZ.X) / (endXZ.X - startXZ.X) : (pos.Y - startXZ.Y) / (endXZ.Y - startXZ.Y);
                    var y = start.Y + t * (end.Y - start.Y);
                    DrillCircleAtPosition(new Vector3(pos.X, y, pos.Y), tool);
                }
            }
            else if(tool.Type == ToolType.Round)
            {
                DrillCircleAtPosition(end, tool);
            }
            else if(InContact(end, tool))
            {
                MessageBox.Show("InvalidMove"); //TODO: replace with exception and handle outside of this class
            }

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
                        if(CheckContactPixel(x, y, _funcList[toolTypeIdx](toolPosition.Y, distSqr, tool.RadiusSqr)))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private float HeightByDistFromCenterRound(float toolY, float distanceSqr, float toolRadiusSqr)
        {
            var remaining = toolRadiusSqr - distanceSqr;
            return toolY - (float)Math.Sqrt(remaining) + (float)Math.Sqrt(toolRadiusSqr);
        }

        private float HeightByDistFromCenterFlat(float toolY, float distanceSqr, float toolRadiusSqr)
        {
            return toolY; //TODO: check if radius should be added/subtracted for both tools
        }

        //void EightWaySymmetricPlot(int xc, int yc, int x, int y, float value)
        //{
        //    putpixel(x + xc, y + yc, RED);
        //    putpixel(x + xc, -y + yc, YELLOW);
        //    putpixel(-x + xc, -y + yc, GREEN);
        //    putpixel(-x + xc, y + yc, YELLOW);
        //    putpixel(y + xc, x + yc, 12);
        //    putpixel(y + xc, -x + yc, 14);
        //    putpixel(-y + xc, -x + yc, 15);
        //    putpixel(-y + xc, x + yc, 6);
        //}

        //void BresenhamCircle(int xc, int yc, int r)
        //{
        //    int x = 0, y = r, d = 3 - (2 * r);
        //    EightWaySymmetricPlot(xc, yc, x, y);

        //    while (x <= y)
        //    {
        //        if (d <= 0)
        //        {
        //            d = d + (4 * x) + 6;
        //        }
        //        else
        //        {
        //            d = d + (4 * x) - (4 * y) + 10;
        //            y = y - 1;
        //        }
        //        x = x + 1;
        //        EightWaySymmetricPlot(xc, yc, x, y);
        //    }
        //}

    }
}
