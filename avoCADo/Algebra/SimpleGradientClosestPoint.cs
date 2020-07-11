using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo.Algebra
{
    public static class SimpleGradientClosestPoint
    {
        public static Vector2 FindStartingPoint(ISurface p, Vector2 x0, Vector3 C, float epsilon)
        {
            int maxIterations = 100;

            var x = x0;
            int i = 0;
            while (SurfaceConditions.ParametersInBounds(p, x) && EpsilonCondition(p, x, C, epsilon) && i < maxIterations)
            {
                var dir = -CalculateGradient(p, x, C);
                x += Alpha(p, x, dir, C, 4.0f) * dir;
                i++;
            }

            if (SurfaceConditions.ParametersInBounds(p, x) == false)
            {
                return new Vector2(float.NaN, float.NaN);
            }
            else return x;
        }


        private static float Alpha(ISurface p, Vector2 xPrev, Vector2 descentDirection, Vector3 C, float startingValue)
        {
            var a = startingValue;
            var fPrev = F(p, xPrev, C);


            float func(float alpha) => F(p, xPrev + alpha * descentDirection, C);
            var val = func(0.0f);

            var eps = 0.00001f;
            a = GoldenSearch(-a, a, func, eps);
            return a;
        }


        private static float _k = (float)((Math.Sqrt(5) - 1.0) * 0.5);

        private static float GoldenSearch(float a, float b, Func<float, float> func, float eps)
        {
            var len = b - a;
            var xl = b - len * _k;
            var xr = a + len * _k;

            while (len > eps)
            {
                if (func(xl) > func(xr))
                {
                    a = xl;
                    len = b - a;
                    xl = xr;
                    xr = a + len * _k;
                }
                else
                {
                    b = xr;
                    len = b - a;
                    xr = xl;
                    xl = b - len * _k;
                }
            }

            return func(xl) > func(xr) ? xr : xl;
        }

        private static float F(ISurface p, Vector2 x, Vector3 C)
        {
            if (SurfaceConditions.ParametersInBounds(p, x) == false) return float.MaxValue;
            var diff = p.GetVertex(x.X, x.Y) - C;
            return Vector3.Dot(diff, diff);
        }

        private static bool EpsilonCondition(ISurface p, Vector2 x, Vector3 C, float epsilon)
        {
            var len = (p.GetVertex(x.X, x.Y) - C).Length;
            return len > epsilon;
        }

        private static Vector2 CalculateGradient(ISurface surfP, Vector2 parameters, Vector3 C)
        {
            //func: F(x) = <P(u, v) - C, P(u, v) - C> (C - point in R3)
            var u = parameters.X;
            var v = parameters.Y;

            var P = surfP.GetVertex(u, v);
            var Pu = surfP.DerivU(u, v);
            var Pv = surfP.DerivV(u, v);

            float du = 2.0f * Vector3.Dot(Pu, (P - C));
            float dv = 2.0f * Vector3.Dot(Pv, (P - C));

            return new Vector2(du, dv);
        }
    }
}
