using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo
{
    public class TridiagonalSolver
    {
        private Vector3[] _lower = new Vector3[0];
        private Vector3[] _center = new Vector3[0];
        private Vector3[] _upper = new Vector3[0];
        private Vector3[] _result = new Vector3[0];
        private float[] _segments = new float[0];
        private float[] _alfas = new float[0];
        private float[] _betas = new float[0];

        private Vector3[] _bernsteinBuffer = new Vector3[4];

        public void Solve(Vector3[] data, IList<Vector3> bernsteins)
        {
            bernsteins.Clear();
            if (data.Length < 2) throw new ArgumentException("problem too small wtf");
            var n = data.Length - 1; //points indexed 0...n (n+1 points)
            //Resize arrays if necessary
            if (_segments.Length != n)
            {
                var n1 = Math.Max(n - 1, 0);
                var n2 = Math.Max(n - 2, 0);
                Array.Resize(ref _result, n1);
                Array.Resize(ref _center, n1);
                Array.Resize(ref _lower, n2);
                Array.Resize(ref _upper, n1);
                Array.Resize(ref _segments, n);
                Array.Resize(ref _alfas, n2);
                Array.Resize(ref _betas, n2);
            }

            //Calculate segment lengths
            var L = 0.0f;
            for (int i = 0; i < n; i++)
            {
                _segments[i] = Math.Max((data[i + 1] - data[i]).Length, 0.000001f); //just so it draws when control points overlap
                L += _segments[i];
            }

            //Calculate alfa and beta arrays (and lower and upper diagonals)
            for(int i = 0; i < n - 2; i++)
            {
                _alfas[i] = _segments[i + 1] / (_segments[i + 1] + _segments[i + 2]); //alfa is one index "later" (we start from alfa2 and beta1)
                _betas[i] = _segments[i + 1] / (_segments[i] + _segments[i + 1]);
                _lower[i] = new Vector3(_alfas[i], _alfas[i], _alfas[i]);
                _upper[i] = new Vector3(_betas[i], _betas[i], _betas[i]);
            }

            //Init main diagonal
            for (int i = 0; i < n - 1; i++)
            {
                _center[i] = new Vector3(2.0f, 2.0f, 2.0f);
            }

            //Init constant term vector
            for(int i = 1; i < n; i++)
            {
                var first = (data[i + 1] - data[i])/_segments[i];
                var second = (data[i] - data[i - 1])/_segments[i-1];
                _result[i-1] = 3.0f * (first - second)/(_segments[i-1] + _segments[i]);
            }

            Solve(_lower, _center, _upper, _result);

            CalculateCoefficients(data, _result, _segments, bernsteins);
        }

        private void CalculateCoefficients(Vector3[] a, Vector3[] result, float[] seg, IList<Vector3> bernsteins)
        {
            var N = a.Length - 1;
            var c = new Vector3[a.Length];
            var b = new Vector3[N];
            var d = new Vector3[N];

            c[0] = c[c.Length - 1] = Vector3.Zero; //N+1 of c
            for(int i = 0; i < result.Length; i++) //TODO: array.copy
            {
                c[i + 1] = result[i];
            }

            for(int i = 1; i < N + 1; i++) //N of d
            {
                d[i - 1] = (c[i] - c[i - 1]) / (3.0f * seg[i - 1]);
            }

            for(int i = 1; i < N + 1; i++) //N of b
            {
                b[i - 1] = (a[i] - a[i - 1] - c[i - 1] * seg[i - 1] * seg[i - 1] - d[i - 1] * seg[i - 1] * seg[i - 1] * seg[i - 1]) / seg[i - 1];
            }

            for(int i = 0; i < N; i++)
            {
                var s1 = seg[i];
                var s2 = s1 * seg[i];
                var s3 = s2 * seg[i];
                MathExtensions.PowerToBernstein(a[i], b[i] * s1, c[i] * s2, d[i] * s3, ref _bernsteinBuffer);
                for(int j = 0; j < 4; j++)
                {
                    if (j != 3 || i == N - 1)
                    {
                        bernsteins.Add(_bernsteinBuffer[j]);
                    }
                }
            }
        }

        private static void Solve(Vector3[] l, Vector3[] d, Vector3[] u, Vector3[] b)
        {
            int X = b.Length;

            if (X == 0) return;
            if (X == 1)
            {
                b[0] = Vector3.Divide(b[0], d[0]);
                return;
            }

            u[0] = Vector3.Divide(u[0], d[0]);
            b[0] = Vector3.Divide(b[0], d[0]);

            /* loop from 1 to X - 1 inclusive, performing the forward sweep */
            for (int ix = 1; ix < X; ix++)
            {
                var m = Vector3.Divide(new Vector3(1.0f, 1.0f, 1.0f), (d[ix] - l[ix - 1] * u[ix - 1]));
                u[ix] = u[ix] * m;
                b[ix] = (b[ix] - l[ix - 1] * b[ix - 1]) * m;
            }

            /* loop from X - 2 to 0 inclusive (safely testing loop condition for an unsigned integer), to perform the back substitution */
            for (int ix = X - 2; ix > 0; ix--)
            {
                b[ix] -= u[ix] * b[ix + 1];
            }
            b[0] -= u[0] * b[1];
        }
    }
}
