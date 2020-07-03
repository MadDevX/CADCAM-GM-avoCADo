using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo.Algebra
{
    public static class SurfaceConditions
    {
        public static bool ParametersInBounds(IntersectionData d, Vector4 x)
        {
            var u = x.X;
            var v = x.Y;
            var s = x.Z;
            var t = x.W;
            var minU = d.p.ParameterURange.X;
            var minV = d.p.ParameterVRange.X;
            var minS = d.q.ParameterURange.X;
            var minT = d.q.ParameterVRange.X;
            var maxU = d.p.ParameterURange.Y;
            var maxV = d.p.ParameterVRange.Y;
            var maxS = d.q.ParameterURange.Y;
            var maxT = d.q.ParameterVRange.Y;

            return
                minU <= u && maxU >= u &&
                minV <= v && maxV >= v &&
                minS <= s && maxS >= s &&
                minT <= t && maxT >= t;
        }
    }
}
