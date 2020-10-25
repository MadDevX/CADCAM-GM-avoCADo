using OpenTK;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo.CNC
{
    public enum ToolType
    {
        Round,
        Flat
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

        public MaterialBlock(int width, int height, float worldWidth, float worldHeight, float defaultValue)
        {
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


        private int GetIndex(int x, int y)
        {
            return x + y * Width;
        }

        private void SetPixel(int x, int y, float value)
        {
            if (x < Width && y < Height)
            {
                HeightMap[GetIndex(x, y)] = value;
            }
        }

        private void DrillPixel(int x, int y, float value)
        {
            if (x < Width && y < Height)
            {
                var idx = GetIndex(x, y);
                HeightMap[idx] = Math.Min(HeightMap[idx], value);
            }
        }

        private (int x, int y) GetCoordsFromPosition(Vector2 worldPosition)
        {
            var localPosition = worldPosition - _blockCenter;
            var offsetPosition = localPosition + _offsetVector;

            var x = (int)(offsetPosition.X * _worldWidthToIndex);
            var y = (int)(offsetPosition.Y * _worldHeightToIndex);

            return (x, y);
        }

        private Vector2 IdxToWorld(int x, int y)
        {
            var wX = x * _indexWidthToWorld;
            var wY = y * _indexHeightToWorld;
            return new Vector2(wX, wY) - _offsetVector;
        }

        public void DrillCircleAtPosition(Vector3 toolPosition, CNCTool tool)
        {
            var radius = tool.Radius;
            var radiusSqr = tool.RadiusSqr;
            var luPos = new Vector2(toolPosition.X - radius, toolPosition.Y - radius);
            var rbPos = new Vector2(toolPosition.X + radius, toolPosition.Y + radius);

            var lu = GetCoordsFromPosition(luPos);
            var rb = GetCoordsFromPosition(rbPos);

            for(int y = lu.y; y < rb.y; y++)
            {
                for(int x = lu.x; x < rb.x; x++)
                {
                    var pos = IdxToWorld(x, y);
                    var offX = toolPosition.X - pos.X;
                    var offY = toolPosition.Y - pos.Y;
                    var distSqr = offX * offX + offY * offY;
                    if (distSqr <= radiusSqr)
                    {
                        DrillPixel(x, y, HeightByDistFromCenter(toolPosition, distSqr, tool));
                    }
                }
            }
        }


        private float HeightByDistFromCenter(Vector3 toolPosition, float distanceSqr, CNCTool tool)
        {
            switch (tool.Type)
            {
                case ToolType.Round:
                    var remaining = tool.RadiusSqr - distanceSqr;
                    return toolPosition.Z - (float)Math.Sqrt(remaining);
                case ToolType.Flat:
                    return toolPosition.Z; //-radius; //TODO: check if radius should be added/subtracted for both tools
                default:
                    return 0.0f;
            }
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
