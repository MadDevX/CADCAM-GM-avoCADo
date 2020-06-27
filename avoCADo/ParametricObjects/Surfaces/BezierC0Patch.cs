using avoCADo.ParametricObjects.Curves;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace avoCADo
{
    public class BezierC0Patch : IBezierSurface
    {
        public Vector2 ParameterURange => new Vector2(0.0f, USegments);

        public Vector2 ParameterVRange => new Vector2(0.0f, VSegments);

        public int USegments => (ControlPoints.Width + 1) / 3;
        public int VSegments => (ControlPoints.Height + 1) / 3;

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

            for(int i = 0; i < 4; i++)
            {
                _coordBuffer[i] = 
                BezierHelper.Bezier3(ControlPoints[uIdx + i, vIdx + 0].Transform.WorldPosition,
                                     ControlPoints[uIdx + i, vIdx + 1].Transform.WorldPosition,
                                     ControlPoints[uIdx + i, vIdx + 2].Transform.WorldPosition,
                                     ControlPoints[uIdx + i, vIdx + 3].Transform.WorldPosition, v);
            }
            return BezierHelper.Bezier3(_coordBuffer[0],
                                        _coordBuffer[1],
                                        _coordBuffer[2],
                                        _coordBuffer[3], u);
        }


        private int GetStartingIndex(int segment, int maxSegments)
        {
            if(segment < maxSegments)
            {
                return segment * 3;
            }
            else
            {
                return (segment - 1) * 3;
            }
        }

        private float GetScaledParameter(float t, int segment, int maxSegments)
        {
            if(segment < maxSegments)
            {
                return t % 1.0f;
            }
            else
            {
                return 1.0f;
            }
        }

        public Vector3 GetTangent(float u, float v)
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
                BezierHelper.Bezier3(ControlPoints[uIdx + i, vIdx + 0].Transform.WorldPosition,
                                     ControlPoints[uIdx + i, vIdx + 1].Transform.WorldPosition,
                                     ControlPoints[uIdx + i, vIdx + 2].Transform.WorldPosition,
                                     ControlPoints[uIdx + i, vIdx + 3].Transform.WorldPosition, v);
            }
            return Vector3.Normalize(BezierHelper.BezierTangent(_coordBuffer[0],
                                                                _coordBuffer[1],
                                                                _coordBuffer[2],
                                                                _coordBuffer[3], u));

        }

        public Vector3 GetBitangent(float u, float v)
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
                BezierHelper.Bezier3(ControlPoints[uIdx + 0, vIdx + i].Transform.WorldPosition,
                                     ControlPoints[uIdx + 1, vIdx + i].Transform.WorldPosition,
                                     ControlPoints[uIdx + 2, vIdx + i].Transform.WorldPosition,
                                     ControlPoints[uIdx + 3, vIdx + i].Transform.WorldPosition, u);
            }
            return Vector3.Normalize(BezierHelper.BezierTangent(_coordBuffer[0],
                                                                _coordBuffer[1],
                                                                _coordBuffer[2],
                                                                _coordBuffer[3], v));

        }

        public Vector3 GetNormal(float u, float v)
        {
            return Vector3.Cross(GetTangent(u, v), GetBitangent(u, v));
        }
    }
}
