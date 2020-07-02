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

        public int Segments => (ControlNodes.Count + 1) / 3;

        public IList<Vector3> ControlPoints => ControlNodes.Select(x => x.Transform.WorldPosition).ToList();

        public IList<INode> ControlNodes { get; }

        //TODO: keep separate list and update it accordingly. 
        public IList<Vector3> BernsteinControlPoints => ControlNodes.Select((x) => x.Transform.WorldPosition).ToList(); //wrong at so many levels

        public IList<Vector3> PolygonPoints => BernsteinControlPoints;

        public BezierC0Curve(IList<INode> controlPointsSource)
        {
            ControlNodes = controlPointsSource;
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
            if (ControlNodes.Count == startIdx + 2)
            {
                return BezierHelper.
                       Bezier(ControlNodes[startIdx].Transform.WorldPosition,
                               ControlNodes[startIdx + 1].Transform.WorldPosition,
                               segmentT
                               );
            }
            if (ControlNodes.Count == startIdx + 3)
            {
                return BezierHelper.
                       Bezier(ControlNodes[startIdx].Transform.WorldPosition,
                               ControlNodes[startIdx + 1].Transform.WorldPosition,
                               ControlNodes[startIdx + 2].Transform.WorldPosition,
                               segmentT
                               );
            }
            if (ControlNodes.Count >= startIdx + 4)
            {
                return BezierHelper.
                       Bezier(ControlNodes[startIdx].Transform.WorldPosition,
                               ControlNodes[startIdx + 1].Transform.WorldPosition,
                               ControlNodes[startIdx + 2].Transform.WorldPosition,
                               ControlNodes[startIdx + 3].Transform.WorldPosition,
                               segmentT
                               );
            }
            throw new InvalidOperationException("invalid parameter value");
        }

        public void Refresh()
        {}
    }
}
