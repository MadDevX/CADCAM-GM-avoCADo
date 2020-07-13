using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace avoCADo.Algebra
{

    public static class ConjugateGradient
    {

        public static Vector4 FindStartingPoint(IntersectionData data, Vector4 x0, float epsilon)
        {
            int maxIterations = 20;

            var r0 = -CalculateGradient(data.p, data.q, x0);
            var p0 = r0;

            var xPrev = x0;
            var pPrev = p0;

            int i = 0;
            while (SurfaceConditions.ParametersInBounds(data, xPrev) && EpsilonCondition(data, xPrev, epsilon) && i < maxIterations)
            {
                var xCur = X(data, x0, p0);
                var pCur = P(data, xPrev, xCur, pPrev);

                xPrev = xCur;
                pPrev = pCur;

                i++;
            }

            if (SurfaceConditions.ParametersInBounds(data, xPrev) == false || i >= maxIterations)
            {
                return new Vector4(float.NaN, float.NaN, float.NaN, float.NaN);
            }
            else return xPrev;
        }

        #region END CONDITIONS

        private static bool EpsilonCondition(IntersectionData d, Vector4 x, float epsilon)
        {
            return (d.p.GetVertex(x.X, x.Y) - d.q.GetVertex(x.Z, x.W)).Length > epsilon;
        }

        #endregion

        #region CONJUGATE GRADIENT METHOD

        private static Vector4 X(IntersectionData d, Vector4 x, Vector4 p)
        {
            return x + Alpha(d, x, p) * p;
        }

        private static float Alpha(IntersectionData d, Vector4 x, Vector4 p)
        {
            var grad = CalculateGradient(d.p, d.q, x);
            var hes = CalculateHessian(d.p, d.q, x);
            return Vector4.Dot(-grad, p) / (Vector4.Dot(p, hes * p));
        }

        private static Vector4 P(IntersectionData d, Vector4 xPrev, Vector4 xCur, Vector4 pPrev)
        {
            return R(d, xCur) + Beta(d, xPrev, xCur) * pPrev;
        }

        private static float Beta(IntersectionData d, Vector4 xPrev, Vector4 xCur)
        {
            var rPrev = R(d, xPrev);
            var rCur = R(d, xCur);
            return Math.Max((Vector4.Dot(rCur, rCur - rPrev))/(Vector4.Dot(rPrev, rPrev)), 0.0f);
        }

        private static Vector4 R(IntersectionData d, Vector4 x)
        {
            return CalculateGradient(d.p, d.q, x);
        }

        #endregion

        #region DERIVATIVES

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

        private static Matrix4 CalculateHessian(ISurface surfP, ISurface surfQ, Vector4 parameters)
        {
            //func: F(x) = <P(u, v) - Q(s, t), P(u, v) - Q(s, t)>
            var u = parameters.X;
            var v = parameters.Y;
            var s = parameters.Z;
            var t = parameters.W;

            var P = surfP.GetVertex(u, v);
            var Pu = surfP.DerivU(u, v);
            var Puu = surfP.DerivUU(u, v);
            var Pv = surfP.DerivV(u, v);
            var Pvv = surfP.DerivVV(u, v);
            var Puv = surfP.Twist(u, v);

            var Q = surfQ.GetVertex(s, t);
            var Qs = surfQ.DerivU(s, t);
            var Qss = surfQ.DerivUU(s, t);
            var Qt = surfQ.DerivV(s, t);
            var Qtt = surfQ.DerivVV(s, t);
            var Qst = surfQ.Twist(s, t);

            Matrix4 hessian = new Matrix4();

            hessian[0, 0] = 2.0f * (Vector3.Dot(Puu, P - Q) + Vector3.Dot(Pu, Pu)); //uu
            hessian[1, 1] = 2.0f * (Vector3.Dot(Pvv, P - Q) + Vector3.Dot(Pv, Pv)); //vv
            hessian[2, 2] = 2.0f * (Vector3.Dot(Qss, Q - P) + Vector3.Dot(Qs, Qs)); //ss
            hessian[3, 3] = 2.0f * (Vector3.Dot(Qtt, Q - P) + Vector3.Dot(Qt, Qt)); //tt

            hessian[0, 1] = hessian[1, 0] = 2.0f * (Vector3.Dot(Puv, P - Q) + Vector3.Dot(Pu, Pv)); //uv
            hessian[0, 2] = hessian[2, 0] = 2.0f * (-Vector3.Dot(Pu, Qs)); //us
            hessian[0, 3] = hessian[3, 0] = 2.0f * (-Vector3.Dot(Pu, Qt)); //ut
            hessian[1, 2] = hessian[2, 1] = 2.0f * (-Vector3.Dot(Pv, Qs)); //vs
            hessian[1, 3] = hessian[3, 1] = 2.0f * (-Vector3.Dot(Pv, Qt)); //vt
            hessian[2, 3] = hessian[3, 2] = 2.0f * (Vector3.Dot(Qst, Q - P) + Vector3.Dot(Qs, Qt)); //st

            return hessian;
        }

        #endregion
    }
}
