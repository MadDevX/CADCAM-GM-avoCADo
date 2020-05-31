using avoCADo.ParametricObjects.Curves;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo
{
    public class BezierC2Patch : IBezierSurface
    {
        public Vector2 ParameterURange => new Vector2(0.0f, USegments);

        public Vector2 ParameterVRange => new Vector2(0.0f, VSegments);

        public int USegments => ControlPoints.Width - 3;
        public int VSegments => ControlPoints.Height - 3;

        public bool ULoop => false; //possibly changeable
        public bool VLoop => false;

        public CoordList<INode> ControlPoints { get; } = new CoordList<INode>();

        public event Action ParametersChanged;

        /// <summary>
        /// Used to store weights for 2-step de Casteljeu algorithm (for surfaces)
        /// </summary>
        private Vector3[] _coordBuffer = new Vector3[4];

        public Vector3 GetVertex(float u, float v)
        {
            int uPatchIdx = (int)u;
            int vPatchIdx = (int)v;
            var uIdx = GetStartingIndex(uPatchIdx, USegments);
            var vIdx = GetStartingIndex(vPatchIdx, VSegments);
            u = GetScaledParameter(u, uPatchIdx, USegments);
            v = GetScaledParameter(v, vPatchIdx, VSegments);

            for (int i = 0; i < 4; i++)
            {
                _coordBuffer[i] =
                BezierHelper.DeBoor(v,
                                    ControlPoints[uIdx + i, vIdx + 0].Transform.WorldPosition,
                                    ControlPoints[uIdx + i, vIdx + 1].Transform.WorldPosition,
                                    ControlPoints[uIdx + i, vIdx + 2].Transform.WorldPosition,
                                    ControlPoints[uIdx + i, vIdx + 3].Transform.WorldPosition);
            }
            return BezierHelper.DeBoor(u,
                                       _coordBuffer[0],
                                       _coordBuffer[1],
                                       _coordBuffer[2],
                                       _coordBuffer[3]);
        }


        private int GetStartingIndex(int segment, int maxSegments)
        {
            if (segment < maxSegments)
            {
                return segment;
            }
            else
            {
                return segment - 1;
            }
        }

        private float GetScaledParameter(float t, int segment, int maxSegments)
        {
            if (segment < maxSegments)
            {
                return t % 1.0f;
            }
            else
            {
                return 1.0f;
            }
        }
    }
}
