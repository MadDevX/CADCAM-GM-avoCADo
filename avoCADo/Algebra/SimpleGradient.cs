using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo.Algebra
{
    public static class SimpleGradient
    {
        public static Vector4 FindStartingPoint(IntersectionData data, Vector4 x0, float epsilon)
        {
            int maxIterations = 100;

            var x = x0;
            int i = 0;
            while (SurfaceConditions.ParametersInBounds(data, x) && EpsilonCondition(data, x, epsilon) && i < maxIterations)
            {
                var dir = -CalculateGradient(data.p, data.q, x);
                x += Alpha(data, x, dir, 4.0f) * dir;
                i++;
            }

            if (SurfaceConditions.ParametersInBounds(data, x) == false || i >= maxIterations)
            {
                return new Vector4(float.NaN, float.NaN, float.NaN, float.NaN);
            }
            else return x;
        }


        private static float Alpha(IntersectionData data, Vector4 xPrev, Vector4 descentDirection, float startingValue)
        {
            var a = startingValue;
            var fPrev = F(data, xPrev);


            float func(float alpha) => F(data, xPrev + alpha * descentDirection);
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

        private static float F(IntersectionData data, Vector4 x)
        {
            var diff = data.p.GetVertex(x.X, x.Y) - data.q.GetVertex(x.Z, x.W);
            return Vector3.Dot(diff, diff);
        }

        private static bool EpsilonCondition(IntersectionData d, Vector4 x, float epsilon)
        {
            var len = (d.p.GetVertex(x.X, x.Y) - d.q.GetVertex(x.Z, x.W)).Length;
            return len > epsilon;
        }

        private static Vector4 CalculateGradient(ISurface surfP, ISurface surfQ, Vector4 parameters)
        {
            //func: F(x) = <P(u, v) - Q(s, t), P(u, v) - Q(s, t)>
            var u = parameters.X;
            var v = parameters.Y;
            var s = parameters.Z;
            var t = parameters.W;

            var P = surfP.GetVertex(u, v);
            var Pu = surfP.DerivU(u, v);
            var Pv = surfP.DerivV(u, v);

            var Q = surfQ.GetVertex(s, t);
            var Qu = surfQ.DerivU(s, t);
            var Qv = surfQ.DerivV(s, t);

            float du = 2.0f * Vector3.Dot(Pu, (P - Q));
            float dv = 2.0f * Vector3.Dot(Pv, (P - Q));
            float ds = 2.0f * Vector3.Dot(Qu, (Q - P));
            float dt = 2.0f * Vector3.Dot(Qv, (Q - P));

            return new Vector4(du, dv, ds, dt);
        }
    }
}
