using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo.ControlPointManagers
{
    public static class CPStartingPositionUtility
    {
        public static void SetControlPointPositions(WrapMode mode, Vector3 position, CoordList<INode> controlPoints, float surfaceWidth, float surfaceHeight)
        {
            switch (mode)
            {
                case WrapMode.None:
                    SetControlPointPoisitionsFlat(position, controlPoints, surfaceWidth, surfaceHeight);
                    break;
                case WrapMode.Column:
                    SetControlPointPoisitionsCylinderColumnWrap(position, controlPoints, surfaceWidth, surfaceHeight);
                    break;
                case WrapMode.Row:
                    SetControlPointPoisitionsCylinderRowWrap(position, controlPoints, surfaceWidth, surfaceHeight);
                    break;
            }
        }

        private static void SetControlPointPoisitionsFlat(Vector3 position, CoordList<INode> controlPoints, float surfaceWidth, float surfaceHeight)
        {
            var width =  controlPoints.Width;
            var height = controlPoints.Height;
            for (int j = 0; j < height; j++)
            {
                for (int i = 0; i < width; i++)
                {
                    controlPoints[i, j].Transform.WorldPosition = position + new Vector3(((float)i / (width - 1)) * surfaceWidth, 0.0f, ((float)j / (height - 1)) * surfaceHeight);
                }
            }
        }

        private static void SetControlPointPoisitionsCylinderColumnWrap(Vector3 position, CoordList<INode> controlPoints, float surfaceRadius, float surfaceHeight)
        {
            var width = controlPoints.DataWidth;
            var height = controlPoints.DataHeight;
            for (int j = 0; j < height; j++)
            {
                for (int i = 0; i < width; i++)
                {
                    var offset = new Vector3(
                            surfaceRadius * (float)Math.Sin(((double)i / (width)) * Math.PI * 2.0),
                            surfaceRadius * (float)Math.Cos(((double)i / (width)) * Math.PI * 2.0),
                            ((float)j / (height - 1)) * surfaceHeight - 0.5f*surfaceHeight);
                    controlPoints[i, j].Transform.WorldPosition = position + offset;
                }
            }
        }

        private static void SetControlPointPoisitionsCylinderRowWrap(Vector3 position, CoordList<INode> controlPoints, float surfaceRadius, float surfaceHeight)
        {
            var width = controlPoints.DataWidth;
            var height = controlPoints.DataHeight;
            for (int j = 0; j < height; j++)
            {
                for (int i = 0; i < width; i++)
                {
                    var offset = new Vector3(
                            ((float)i / (width - 1)) * surfaceHeight - 0.5f * surfaceHeight,
                            surfaceRadius * (float)Math.Cos(((double)j / (height)) * Math.PI * 2.0),
                            surfaceRadius * (float)Math.Sin(((double)j / (height)) * Math.PI * 2.0)
                            );
                    controlPoints[i, j].Transform.WorldPosition = position + offset;
                }
            }
        }
    }
}
