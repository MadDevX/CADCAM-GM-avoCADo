using OpenTK;
using avoCADo.Algebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo.Intersections
{
    public static class BoundaryFinder
    {
        public static Vector4 FindBoundaryPoint(IntersectionData data, Vector4 x0, Vector4 xOut, float epsilon = 0.00001f)
        {
            var a = x0; //a is inside bounds
            var b = xOut; //b is outside bounds
            while ((a - b).Length > epsilon)
            {
                var mid = Vector4.Lerp(a, b, 0.5f);
                if (SurfaceConditions.ParametersInBounds(data, mid) == false) b = mid;
                else a = mid;
            }
            return a;
        }

    }
}
