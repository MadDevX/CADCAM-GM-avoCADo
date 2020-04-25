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

        public Vector3[] Solve(Vector3[] data)
        {
            if (data.Length < 2) throw new ArgumentException("problem too small wtf");
            var n = data.Length;
            //Resize arrays if necessary
            if (_center.Length != n)
            {
                Array.Resize(ref _result, n);
                Array.Resize(ref _center, n);
                Array.Resize(ref _lower, n - 1);
                Array.Resize(ref _upper, n);
            }
            //Init tridiagonal matrix
            for (int i = 0; i < n - 1; i++)
            {
                _lower[i] = _upper[i] = new Vector3(1.0f, 1.0f, 1.0f);
                _center[i + 1] = new Vector3(4.0f, 4.0f, 4.0f);
            }
            _center[0] = _center[n - 1] = new Vector3(2.0f, 2.0f, 2.0f);

            var L = 0.0f;
            for (int i = 0; i < n-1; i++)
            {
                L += (data[i + 1] - data[i]).Length;
            }

            //Init constant term vector
            _result[0] = 3.0f * (data[1] - data[0]);
            for(int i = 1; i < n - 1; i++)
            {
                _result[i] = 3.0f * (data[i + 1] - data[i - 1]);
            }
            _result[n - 1] = 3.0f * (data[n - 1] - data[n - 2]);

            Solve(_lower, _center, _upper, _result);
            return _result;
        }

        private static void Solve(Vector3[] a, Vector3[] b, Vector3[] c, Vector3[] x)
        {
            uint X = (uint)x.Length;
            c[0] = Vector3.Divide(c[0], b[0]);
            x[0] = Vector3.Divide(x[0], b[0]);

            /* loop from 1 to X - 1 inclusive, performing the forward sweep */
            for (uint ix = 1; ix < X; ix++)
            {
                var m = Vector3.Divide(new Vector3(1.0f,1.0f,1.0f), (b[ix] - a[ix - 1] * c[ix - 1]));
                c[ix] = c[ix] * m;
                x[ix] = (x[ix] - a[ix - 1] * x[ix - 1]) * m;
            }

            /* loop from X - 2 to 0 inclusive (safely testing loop condition for an unsigned integer), to perform the back substitution */
            for (uint ix = X - 2; ix > 0; ix--)
            {
                x[ix] -= c[ix] * x[ix + 1];
            }
            x[0] -= c[0] * x[1];
        }
    }
}
