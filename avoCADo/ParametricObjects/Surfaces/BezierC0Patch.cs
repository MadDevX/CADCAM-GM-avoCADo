﻿using avoCADo.ParametricObjects;
using avoCADo.ParametricObjects.Curves;
using avoCADo.Trimming;
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

        public bool ULoop => ControlPoints.DataWidth != ControlPoints.Width;
        public bool VLoop => ControlPoints.DataHeight != ControlPoints.Height;
        public IList<IntersectionCurveData> BoundingCurves { get; } = new List<IntersectionCurveData>();
        public TrimTextureProvider TrimTexture { get; }

        public CoordList<INode> ControlPoints { get; } = new CoordList<INode>();

        public event Action ParametersChanged;

        /// <summary>
        /// Used to store weights for 2-step de Casteljeu algorithm (for surfaces)
        /// </summary>
        private Vector3[] _coordBuffer = new Vector3[4];

        public BezierC0Patch()
        {
            TrimTexture = new TrimTextureProvider(this);
        }


        public Vector3 GetVertex(float u, float v)
        {
            var uv = ParameterHelper.CorrectUV(this, new Vector2(u, v));
            u = uv.X;
            v = uv.Y;
            int uPatchIdx = (int)u;
            int vPatchIdx = (int)v;
            var uIdx = GetStartingIndex(uPatchIdx, USegments);
            var vIdx = GetStartingIndex(vPatchIdx, VSegments);
            u = GetScaledParameter(u, uPatchIdx, USegments);
            v = GetScaledParameter(v, vPatchIdx, VSegments);

            for(int i = 0; i < 4; i++)
            {
                _coordBuffer[i] = 
                BezierHelper.Bezier(ControlPoints[uIdx + i, vIdx + 0].Transform.WorldPosition,
                                     ControlPoints[uIdx + i, vIdx + 1].Transform.WorldPosition,
                                     ControlPoints[uIdx + i, vIdx + 2].Transform.WorldPosition,
                                     ControlPoints[uIdx + i, vIdx + 3].Transform.WorldPosition, v);
            }
            return BezierHelper.Bezier(_coordBuffer[0],
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

        public Vector3 DerivU(float u, float v) => Deriv(u, v, true, 1);

        public Vector3 DerivUU(float u, float v) => Deriv(u, v, true, 2);

        public Vector3 DerivV(float u, float v) => Deriv(u, v, false, 1);

        public Vector3 DerivVV(float u, float v) => Deriv(u, v, false, 2);

        private Vector3 Deriv(float u, float v, bool uDeriv, int order)
        {
            var uv = ParameterHelper.CorrectUV(this, new Vector2(u, v));
            u = uv.X;
            v = uv.Y;
            int uPatchIdx = (int)u;
            int vPatchIdx = (int)v;
            var uIdx = GetStartingIndex(uPatchIdx, USegments);
            var vIdx = GetStartingIndex(vPatchIdx, VSegments);
            u = GetScaledParameter(u, uPatchIdx, USegments);
            v = GetScaledParameter(v, vPatchIdx, VSegments);

            if (uDeriv)
            {
                for (int i = 0; i < 4; i++)
                {
                    _coordBuffer[i] =
                    BezierHelper.Bezier(ControlPoints[uIdx + i, vIdx + 0].Transform.WorldPosition,
                                         ControlPoints[uIdx + i, vIdx + 1].Transform.WorldPosition,
                                         ControlPoints[uIdx + i, vIdx + 2].Transform.WorldPosition,
                                         ControlPoints[uIdx + i, vIdx + 3].Transform.WorldPosition, v);
                }
                return BezierHelper.BezierDerivative(u, order, 
                                                  _coordBuffer[0],
                                                  _coordBuffer[1],
                                                  _coordBuffer[2],
                                                  _coordBuffer[3]);
            }
            else
            {
                for (int i = 0; i < 4; i++)
                {
                    _coordBuffer[i] =
                    BezierHelper.Bezier(ControlPoints[uIdx + 0, vIdx + i].Transform.WorldPosition,
                                         ControlPoints[uIdx + 1, vIdx + i].Transform.WorldPosition,
                                         ControlPoints[uIdx + 2, vIdx + i].Transform.WorldPosition,
                                         ControlPoints[uIdx + 3, vIdx + i].Transform.WorldPosition, u);
                }
                return BezierHelper.BezierDerivative(v, order, 
                                                  _coordBuffer[0],
                                                  _coordBuffer[1],
                                                  _coordBuffer[2],
                                                  _coordBuffer[3]);
            }

        }

        public Vector3 Normal(float u, float v)
        {
            var uv = ParameterHelper.CorrectUV(this, new Vector2(u, v));
            u = uv.X;
            v = uv.Y;
            return Vector3.Cross(DerivU(u, v), DerivV(u, v)).Normalized();
        }

        public Vector3 Twist(float u, float v)
        {
            var uv = ParameterHelper.CorrectUV(this, new Vector2(u, v));
            u = uv.X;
            v = uv.Y;
            int uPatchIdx = (int)u;
            int vPatchIdx = (int)v;
            var uIdx = GetStartingIndex(uPatchIdx, USegments);
            var vIdx = GetStartingIndex(vPatchIdx, VSegments);
            u = GetScaledParameter(u, uPatchIdx, USegments);
            v = GetScaledParameter(v, vPatchIdx, VSegments);
            for (int i = 0; i < 4; i++)
            {
                _coordBuffer[i] =
                BezierHelper.BezierDerivative(v, 1,
                                                ControlPoints[uIdx + i, vIdx + 0].Transform.WorldPosition,
                                                ControlPoints[uIdx + i, vIdx + 1].Transform.WorldPosition,
                                                ControlPoints[uIdx + i, vIdx + 2].Transform.WorldPosition,
                                                ControlPoints[uIdx + i, vIdx + 3].Transform.WorldPosition);
            }
            return BezierHelper.BezierDerivative(u, 1,
                                                _coordBuffer[0],
                                                _coordBuffer[1],
                                                _coordBuffer[2],
                                                _coordBuffer[3]);
        }

        public bool DifferentPatches(float u1, float v1, float u2, float v2)
        {
            var uv1 = ParameterHelper.CorrectUV(this, new Vector2(u1, v1));
            u1 = uv1.X;
            v1 = uv1.Y;
            int u1PatchIdx = (int)u1;
            int v1PatchIdx = (int)v1;

            var uv2 = ParameterHelper.CorrectUV(this, new Vector2(u2, v2));
            u2 = uv2.X;
            v2 = uv2.Y;
            int u2PatchIdx = (int)u2;
            int v2PatchIdx = (int)v2;

            return u1PatchIdx != u2PatchIdx || v1PatchIdx != v2PatchIdx;
        }
    }
}
