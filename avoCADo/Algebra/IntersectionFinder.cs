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
        private const float _domainSamplingMult = 0.1f;
        private const float _discardLength = 0.1f;

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
                            if ((data.p.GetVertex(u, v) - data.q.GetVertex(s, t)).Length > _discardLength) continue;

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
            //TODO: find Vector4 of parameters closest to startingReferencePoint
            throw new NotImplementedException();
        }

        public static IList<Vector4> FindIntersection(IntersectionData data, Vector4 startingParameters, float knotDistance)
        {
            var x0 = SimpleGradient.FindStartingPoint(data, startingParameters, _epsilon);
            if (float.IsNaN(x0.X)) return null;

            var loop = NewtonMethod.CalculateIntersectionPoints(data, x0, knotDistance, _epsilon);

            return loop;
        }

    }
}
