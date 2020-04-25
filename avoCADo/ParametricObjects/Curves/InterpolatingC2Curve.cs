using avoCADo.ParametricObjects.Curves;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo
{
    public class InterpolatingC2Curve : ICurve
    {
        public int Segments => ControlPoints.Count - 1;

        public Vector2 ParameterRange => new Vector2(0.0f, Segments);

        public IList<INode> ControlPoints { get; }

        public IList<Vector3> BernsteinControlPoints { get; } = new List<Vector3>();

        private TridiagonalSolver _solver;

        private Vector3[] _bernsteinBuffer = new Vector3[4];

        public InterpolatingC2Curve(IList<INode> knotList)
        {
            _solver = new TridiagonalSolver();
            ControlPoints = knotList;
        }


        public Vector3 GetVertex(float t)
        {
            int segment = (int)t;
            int startIdx;
            float segmentT;
            if (segment < Segments)
            {
                startIdx = segment * 3;
                segmentT = t % 1.0f;
            }
            else
            {
                startIdx = (segment - 1) * 3;
                segmentT = 1.0f;
            }
            if (BernsteinControlPoints.Count >= startIdx + 4)
            {
                return BezierHelper.Bezier3(
                               BernsteinControlPoints[startIdx],
                               BernsteinControlPoints[startIdx + 1],
                               BernsteinControlPoints[startIdx + 2],
                               BernsteinControlPoints[startIdx + 3],
                               segmentT
                               );
            }
            throw new Exception("invalid parameter value");
        }

        public void Refresh()
        {
            BernsteinControlPoints.Clear();
            if (ControlPoints.Count < 2) return;

            var knots = ControlPoints.Select((x) => x.Transform.WorldPosition).ToArray();
            //Calculate D vector
            var dArray = _solver.Solve(knots);
            List<Vector4> coefficients = new List<Vector4>();
            for(int i = 0; i < dArray.Length - 1; i++)
            {
                var a = knots[i];
                var b = dArray[i];
                var c = 3.0f * (knots[i + 1] - knots[i]) - 2.0f * dArray[i] - dArray[i + 1];
                var d = 2.0f * (knots[i] - knots[i + 1]) + dArray[i] + dArray[i + 1];
                PowerToBernstein(a, b, c, d, ref _bernsteinBuffer);
                for (int j = 0; j < 4; j++)
                {
                    if (j != 3 || i == dArray.Length - 2)
                    {
                        BernsteinControlPoints.Add(_bernsteinBuffer[j]);
                    }
                }
            }
        }

        private void PowerToBernstein(Vector3 a, Vector3 b, Vector3 c, Vector3 d, ref Vector3[] result)
        {
            var x = new Vector4(a.X, b.X, c.X, d.X);
            var y = new Vector4(a.Y, b.Y, c.Y, d.Y);
            var z = new Vector4(a.Z, b.Z, c.Z, d.Z);

            x = MathExtensions.PowerToBernstein(x);
            y = MathExtensions.PowerToBernstein(y);
            z = MathExtensions.PowerToBernstein(z);
            
            result[0] = new Vector3(x.X, y.X, z.X);
            result[1] = new Vector3(x.Y, y.Y, z.Y);
            result[2] = new Vector3(x.Z, y.Z, z.Z);
            result[3] = new Vector3(x.W, y.W, z.W);

        }
    }
}
