using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using avoCADo.ParametricObjects.Curves;

namespace avoCADo
{
    class BezierC2Curve : ICurve, IVirtualControlPoints
    {
        public Vector2 ParameterRange => new Vector2(0.0f, Segments);

        public int Segments
        {
            get
            {
                if (ControlPoints.Count < 4) return 0;
                return ((1 + 3 * (ControlPoints.Count - 3)) + 1) / 3;
            }
        }

        public IList<INode> ControlPoints { get; }

        public IList<Vector3> BernsteinControlPoints { get; } = new List<Vector3>();

        public IList<Vector3> VirtualControlPoints { get; } = new List<Vector3>();

        public BezierC2Curve(IList<INode> controlPointsSource)
        {
            ControlPoints = controlPointsSource;
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

        private void CalculateBernsteinControlPoints()
        {
            VirtualControlPoints.Clear();
            BernsteinControlPoints.Clear();
            var deBoorPoints = ControlPoints;
            if(deBoorPoints.Count > 3)
            {
                VirtualControlPoints.Add(Vector3.Lerp(deBoorPoints[0].Transform.WorldPosition, deBoorPoints[1].Transform.WorldPosition, 2.0f / 3.0f));
            }
            for (int i = 0; i < deBoorPoints.Count - 3; i++)
            {
                AddBernstein(BernsteinControlPoints, deBoorPoints[i + 0].Transform.WorldPosition,
                                              deBoorPoints[i + 1].Transform.WorldPosition,
                                              deBoorPoints[i + 2].Transform.WorldPosition,
                                              deBoorPoints[i + 3].Transform.WorldPosition,
                                              i == deBoorPoints.Count - 4
                                              );
            }
            if(deBoorPoints.Count > 3)
            {
                for(int i = 0; i < BernsteinControlPoints.Count; i++)
                {
                    VirtualControlPoints.Add(BernsteinControlPoints[i]);
                }
                VirtualControlPoints.Add(Vector3.Lerp(deBoorPoints[deBoorPoints.Count - 2].Transform.WorldPosition,
                                                      deBoorPoints[deBoorPoints.Count - 1].Transform.WorldPosition,
                                                      1.0f / 3.0f));
            }

        }

        private void AddBernstein(IList<Vector3> bernsteinPointsList, Vector3 a, Vector3 b, Vector3 c, Vector3 d, bool addLast)
        {
            var oneThird = 1.0f / 3.0f;
            var twoThirds = 2.0f / 3.0f;

            var firstMid = Vector3.Lerp(a, b, twoThirds);
            var secondMid = Vector3.Lerp(b, c, oneThird);
            var thirdMid = Vector3.Lerp(b, c, twoThirds);
            var fourthMid = Vector3.Lerp(c, d, oneThird);
            bernsteinPointsList.Add(Vector3.Lerp(firstMid, secondMid, 0.5f));
            bernsteinPointsList.Add(secondMid);
            bernsteinPointsList.Add(thirdMid);
            if (addLast) bernsteinPointsList.Add(Vector3.Lerp(thirdMid, fourthMid, 0.5f));
        }

        public void Refresh()
        {
            CalculateBernsteinControlPoints();
        }
    }
}
