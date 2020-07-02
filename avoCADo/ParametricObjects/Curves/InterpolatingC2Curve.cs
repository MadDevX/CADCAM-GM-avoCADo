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
        public int Segments => ControlNodes.Count - 1;

        public Vector2 ParameterRange => new Vector2(0.0f, Segments);

        public IList<Vector3> ControlPoints => ControlNodes.Select(x => x.Transform.WorldPosition).ToList(); //TODO: optimize list creation
        public IList<INode> ControlNodes { get; }

        public IList<Vector3> BernsteinControlPoints { get; } = new List<Vector3>();

        public IList<Vector3> PolygonPoints => ControlNodes.Select((x)=>x.Transform.WorldPosition).ToList();


        private TridiagonalSolver _solver;

        private Vector3[] _bernsteinBuffer = new Vector3[4];

        public InterpolatingC2Curve(IList<INode> knotList)
        {
            _solver = new TridiagonalSolver();
            ControlNodes = knotList;
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
                return BezierHelper.Bezier(
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

            if (ControlNodes.Count < 2) return;

            var knots = ControlNodes.Select((x) => x.Transform.WorldPosition).ToArray();

            _solver.Solve(knots, BernsteinControlPoints);
        }
    }
}
