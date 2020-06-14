using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace avoCADo.ParametricObjects.Curves
{
    public static class BezierHelper
    {
        public static List<Vector3> SplitBezier(Vector3 a, Vector3 b, Vector3 c, Vector3 d, float t)
        {
            var dividedCPs = new List<Vector3>();
            dividedCPs.Add(a);
            dividedCPs.Add(Bezier1(a, b, t));
            dividedCPs.Add(Bezier2(a, b, c, t));
            dividedCPs.Add(Bezier3(a, b, c, d, t));
            dividedCPs.Add(Bezier2(b, c, d, t));
            dividedCPs.Add(Bezier1(c, d, t));
            dividedCPs.Add(d);
            return dividedCPs;
        }

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

        public static Vector3 DeBoor(float t, params Vector3[] p)
        {
            Vector3 r = Vector3.Zero;
            int n = p.GetLength(0);
            if (n < 4) return r;
            int j = (int)(t * (n - 3));
            if (j > n - 4) j = n - 4;
            for (int i = j; i <= j + 3; i++)
            {
                r += N(n, i, t) * p[i];
            }
            return r;
        }

        private static float B(int n, float t)
        {
            var sixth = 1.0f / 6.0f;
            var t1 = 1.0f - t;
            if (n == 0) return sixth * t1 * t1 * t1;
            if (n == 1) return sixth * (3.0f * t * t * t - 6.0f * t * t + 4.0f);
            if (n == 2) return sixth * (-3.0f * t * t * t + 3.0f * t * t + 3.0f * t + 1.0f);
            if (n == 3) return sixth * t * t * t;
            return 0.0f;
        }

        private static float N(int n, int i, float t)
        {
            float tj, T = t * (n - 3);
            int b = -1;
            if ((i - 3.0f <= T) && (T < i - 2.0f)) b = 3;
            if ((i - 2.0f <= T) && (T < i - 1.0f)) b = 2;
            if ((i - 1.0f <= T) && (T < i)) b = 1;
            if ((i <= T) && (T < i + 1.0f)) b = 0;
            if (b == -1) return 0.0f;
            tj = T - i + b;
            return B(b, tj);
        }
    }
}
