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
            dividedCPs.Add(Bezier(a, b, t));
            dividedCPs.Add(Bezier(a, b, c, t));
            dividedCPs.Add(Bezier(a, b, c, d, t));
            dividedCPs.Add(Bezier(b, c, d, t));
            dividedCPs.Add(Bezier(c, d, t));
            dividedCPs.Add(d);
            return dividedCPs;
        }

        public static Vector3 Bezier(Vector3 a, Vector3 b, float t)
        {
            return Vector3.Lerp(a, b, t);
        }

        public static Vector3 Bezier(Vector3 a, Vector3 b, Vector3 c, float t)
        {
            return Vector3.Lerp(Bezier(a, b, t), Bezier(b, c, t), t);
        }

        public static Vector3 Bezier(Vector3 a, Vector3 b, Vector3 c, Vector3 d, float t)
        {
            return Vector3.Lerp(Bezier(a, b, c, t), Bezier(b, c, d, t), t);
        }

        public static Vector3 BezierTangent(Vector3 a, Vector3 b, Vector3 c, Vector3 d, float t)
        {
            return 3.0f * (Bezier(b, c, d, t) - Bezier(a, b, c, t));
        }

        public static Vector3 BezierTangent(Vector3 a, Vector3 b, Vector3 c, float t)
        {
            return 2.0f * (Bezier(b, c, t) - Bezier(a, b, t));
        }

        public static Vector3 BezierTangent(Vector3 a, Vector3 b, float t)
        {
            return b - a;
        }

        public static Vector3 DeboorDerivative(float t, int order, params Vector3[] cps)
        {
            if (cps.Length != 4) throw new InvalidOperationException("Only cubic splines are properly handled, 4 control points expected");
            var bezier = MathExtensions.BSplineToBernstein(cps[0], cps[1], cps[2], cps[3]);
            return BezierDerivative(t, order, bezier.a, bezier.b, bezier.c, bezier.d);
        }

        public static Vector3 BezierDerivative(float t, int order, params Vector3[] cps)
        {
            var n = cps.Length - 1;
            if (order > n) return Vector3.Zero;
            for(int i = 0; i < order; i++)
            {
                var degree = n - i;
                for(int j = 0; j < n - i; j++)
                {
                    cps[j] = (cps[j + 1] - cps[j]) * degree;
                }
            }
            return Bezier(t, cps.SubArray(0, cps.Length - order));
        }

        private static Vector3 Bezier(float t, params Vector3[] cps)
        {
            if (cps.Length == 1) return cps[0];
            var len = cps.Length;
            return Vector3.Lerp(Bezier(t, cps.SubArray(0, len - 1)), Bezier(t, cps.SubArray(1, len - 1)), t);
        }

        public static Vector3 DeBoorTangent(Vector3 a, Vector3 b, Vector3 c, Vector3 d, float t)
        {
            var bezier = MathExtensions.BSplineToBernstein(a, b, c, d);
            return BezierTangent(bezier.a, bezier.b, bezier.c, bezier.d, t);
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
