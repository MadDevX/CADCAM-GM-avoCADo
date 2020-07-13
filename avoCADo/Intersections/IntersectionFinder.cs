using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo.Algebra
{
    public static class IntersectionFinder
    {
        private const float _epsilon = 0.0001f;
        private const float _projectionEpsilon = 0.05f;
        private const float _domainSamplingMult = 0.05f;
        private const float _domainSamplingProjectionMult = 0.1f;
        private const float _discardLength = 0.5f;

        public static IList<Vector4> FindIntersection(IntersectionData data, float knotDistance)
        {
            var pURng = data.p.ParameterURange;
            var pVRng = data.p.ParameterVRange;
            var qSRng = data.q.ParameterURange;
            var qTRng = data.q.ParameterVRange;

            var uLen = pURng.Y - pURng.X;
            var vLen = pVRng.Y - pVRng.X;
            var sLen = qSRng.Y - qSRng.X;
            var tLen = qTRng.Y - qTRng.X;


            for(var u = pURng.X; u <= pURng.Y; u += uLen * _domainSamplingMult)
            {
                for (var v = pVRng.X; v <= pVRng.Y; v += vLen * _domainSamplingMult)
                {
                    for (var s = qSRng.X; s <= qSRng.Y; s += sLen * _domainSamplingMult)
                    {
                        for (var t = qTRng.X; t <= qTRng.Y; t += tLen * _domainSamplingMult)
                        {
                            //if ((data.p.GetVertex(u, v) - data.q.GetVertex(s, t)).Length > _discardLength) continue;

                            var startingParameters = new Vector4(u, v, s, t);
                            var res = FindIntersection(data, startingParameters, knotDistance);
                            if (res != null) return res;
                        }
                    }
                }
            }

            return null;
        }

        public static IList<Vector4> FindIntersection(IntersectionData data, Vector3 startingReferencePoint, float knotDistance)
        {
            var uv = FindClosestPoint(data.p, startingReferencePoint);
            var st = FindClosestPoint(data.q, startingReferencePoint);

            return FindIntersection(data, new Vector4(uv.X, uv.Y, st.X, st.Y), knotDistance);
        }

        private static Vector2 FindClosestPoint(ISurface p, Vector3 startingReferencePoint)
        {
            var pURng = p.ParameterURange;
            var pVRng = p.ParameterVRange;
            var uLen = pURng.Y - pURng.X;
            var vLen = pVRng.Y - pVRng.X;
            var minDist = float.MaxValue;
            Vector2 minUV = new Vector2(pURng.X + uLen * 0.5f, pVRng.X + vLen * 0.5f);
            for (var u = pURng.X; u <= pURng.Y; u += uLen * _domainSamplingProjectionMult)
            {
                for (var v = pVRng.X; v <= pVRng.Y; v += vLen * _domainSamplingProjectionMult)
                {
                    var uv = SimpleGradientClosestPoint.FindStartingPoint(p, new Vector2(u, v), startingReferencePoint, _epsilon); //FindClosestToP
                    if (float.IsNaN(uv.X)) continue;
                    var pnt = p.GetVertex(uv.X, uv.Y);
                    var dist = (pnt - startingReferencePoint).Length;
                    if (dist < minDist)
                    {
                        minDist = dist;
                        minUV = uv;
                        if (minDist < _projectionEpsilon) return minUV;
                    }
                }
            }
            return minUV;
        }

        private static IList<Vector4> FindIntersection(IntersectionData data, Vector4 startingParameters, float knotDistance)
        {
            var x0 = SimpleGradient.FindStartingPoint(data, startingParameters, _epsilon);
            if (float.IsNaN(x0.X)) return null;
            if (data.p == data.q)
            {
                var len = (x0.Xy - x0.Zw).LengthSquared;
                if (len < _epsilon * 1.0f)
                {
                    return null;
                }
            }

            var loop = NewtonMethod.CalculateIntersectionPoints(data, x0, knotDistance, _epsilon);

            return loop;
        }

    }
}
