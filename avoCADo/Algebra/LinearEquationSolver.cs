using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo.Algebra
{
    using real = Double;
    using Matrix = Matrix4d;
    using Vector = Vector4d;

    public static class LinearEquationSolver
    {

        public static Vector4 Solve(Matrix4 A, Vector4 b)
        {
            var ret = Solve(ConvertMat(A), ConvertVec(b));
            return new Vector4((float)ret.X, (float)ret.Y, (float)ret.Z, (float)ret.W);
        }

        private static Vector Solve(Matrix A, Vector b)
        {
            for (int i = 0; i < 4; i++)
            {
                var idx = FindMaxAbsInColumn(A, i, i);
                if (idx == -1) throw new InvalidOperationException("multiple/none solutions exist");
                SwapRows(ref A, ref b, idx, i);
                for (int j = i + 1; j < 4; j++)
                {
                    SubstractRows(ref A, ref b, j, i, A[j, i] / A[i, i]);
                }

            }

            for (int i = 3; i >= 0; i--)//subtrahend (column)
            {
                for (int j = 0; j < i; j++)//minunend 
                {
                    SubstractRows(ref A, ref b, i, j, A[j, i] / A[i, i]);
                }
            }

            for (int i = 0; i < 4; i++)
            {
                MultiplyRow(ref A, ref b, i, 1.0f / A[i, i]);
            }

            return b;
        }

        private static void SwapRows(ref Matrix mat, ref Vector v, int rowA, int rowB)
        {
            for(int i = 0; i < 4; i++)
            {
                var temp = mat[rowA, i];
                mat[rowA, i] = mat[rowB, i];
                mat[rowB, i] = temp;
            }
            var tempV = v[rowA];
            v[rowA] = v[rowB];
            v[rowB] = tempV;
        }

        private static void MultiplyRow(ref Matrix mat, ref Vector v, int row, real mult)
        {
            for(int i = 0; i < 4; i++)
            {
                mat[row, i] *= mult;
            }
            v[row] *= mult;
        }

        private static void SubstractRows(ref Matrix mat, ref Vector v, int minunendRow, int subtrahendRow, real subtrahendMult = 1.0)
        {
            for(int i = 0; i < 4; i++)
            {
                mat[minunendRow, i] -= mat[subtrahendRow, i] * subtrahendMult;
            }
            v[minunendRow] -= v[subtrahendRow] * subtrahendMult;
        }

        /// <summary>
        /// Returns -1 if all elements in column are 0
        /// </summary>
        /// <param name="A"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        private static int FindMaxAbsInColumn(Matrix A, int column, int minRow)
        {
            real curMax = 0.0;
            int idx = -1;

            for(int i = minRow; i < 4; i++)
            {
                var abs = Math.Abs(A[i, column]);
                if (abs > curMax)
                {
                    curMax = abs;
                    idx = i;
                }
            }
            return idx;
        }

        private static Matrix ConvertMat(Matrix4 a)
        {
            return new Matrix(
                a[0, 0], a[0, 1], a[0, 2], a[0, 3],
                a[1, 0], a[1, 1], a[1, 2], a[1, 3],
                a[2, 0], a[2, 1], a[2, 2], a[2, 3],
                a[3, 0], a[3, 1], a[3, 2], a[3, 3]
                );
        }

        private static Vector ConvertVec(Vector4 vec)
        {
            return new Vector(vec.X, vec.Y, vec.Z, vec.W);
        }
    }
}
