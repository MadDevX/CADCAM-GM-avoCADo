using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo.ParametricObjects.Curves
{
    public static class BezierHelper
    {
        public static Vector3 Bezier1(Vector3 a, Vector3 b, float t)
        {
            return Vector3.Lerp(a, b, t);
        }

        public static Vector3 Bezier2(Vector3 a, Vector3 b, Vector3 c, float t)
        {
            return Vector3.Lerp(Bezier1(a, b, t), Bezier1(b, c, t), t);
        }

        public static Vector3 Bezier3(Vector3 a, Vector3 b, Vector3 c, Vector3 d, float t)
        {
            return Vector3.Lerp(Bezier2(a, b, c, t), Bezier2(b, c, d, t), t);
        }
    }
}
