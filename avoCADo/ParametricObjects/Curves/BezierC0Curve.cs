using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using avoCADo.ParametricObjects.Curves;

namespace avoCADo
{
    class BezierC0Curve : ICurve
    {
        public Vector2 ParameterRange => new Vector2(0.0f, Segments);

        public int Segments => (ControlPoints.Count + 1) / 3;

        public IList<INode> ControlPoints { get; }

        //TODO: keep separate list and update it accordingly. 
        public IList<Vector3> BernsteinControlPoints => ControlPoints.Select((x) => x.Transform.WorldPosition).ToList(); //wrong at so many levels

        public BezierC0Curve(IList<INode> controlPointsSource)
        {
            ControlPoints = controlPointsSource;
        }

        public Vector3 GetVertex(float t)
        {
            int segment = (int)t;
            int startIdx;
            float segmentT;
            if(segment < Segments)
            { 
                startIdx = segment * 3;
                segmentT = t % 1.0f;
            }
            else
            {
                startIdx = (segment - 1) * 3;
                segmentT = 1.0f;
            }
            if (ControlPoints.Count == startIdx + 2)
            {
                return BezierHelper.
                       Bezier1(ControlPoints[startIdx].Transform.WorldPosition,
                               ControlPoints[startIdx + 1].Transform.WorldPosition,
                               segmentT
                               );
            }
            if (ControlPoints.Count == startIdx + 3)
            {
                return BezierHelper.
                       Bezier2(ControlPoints[startIdx].Transform.WorldPosition,
                               ControlPoints[startIdx + 1].Transform.WorldPosition,
                               ControlPoints[startIdx + 2].Transform.WorldPosition,
                               segmentT
                               );
            }
            if (ControlPoints.Count >= startIdx + 4)
            {
                return BezierHelper.
                       Bezier3(ControlPoints[startIdx].Transform.WorldPosition,
                               ControlPoints[startIdx + 1].Transform.WorldPosition,
                               ControlPoints[startIdx + 2].Transform.WorldPosition,
                               ControlPoints[startIdx + 3].Transform.WorldPosition,
                               segmentT
                               );
            }
            throw new InvalidOperationException("invalid parameter value");
        }

        public void Refresh()
        {}
    }
}
