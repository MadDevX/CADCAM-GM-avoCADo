using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo
{
    class BezierC0Curve : ICurve
    {
        public Vector2 ParameterRange => new Vector2(0.0f, Segments);

        private int Segments => (ControlPoints.Count + 1) / 3;

        public IList<INode> ControlPoints { get; }

        public IList<Vector3> ControlPointsPositions => ControlPoints.Select((x) => x.Transform.WorldPosition).ToList(); //wrong at so many levels

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
                return Bezier1(ControlPoints[startIdx].Transform.WorldPosition,
                               ControlPoints[startIdx + 1].Transform.WorldPosition,
                               segmentT
                               );
            }
            if (ControlPoints.Count == startIdx + 3)
            {
                return Bezier2(ControlPoints[startIdx].Transform.WorldPosition,
                               ControlPoints[startIdx + 1].Transform.WorldPosition,
                               ControlPoints[startIdx + 2].Transform.WorldPosition,
                               segmentT
                               );
            }
            if (ControlPoints.Count >= startIdx + 4)
            {
                return Bezier3(ControlPoints[startIdx].Transform.WorldPosition,
                               ControlPoints[startIdx + 1].Transform.WorldPosition,
                               ControlPoints[startIdx + 2].Transform.WorldPosition,
                               ControlPoints[startIdx + 3].Transform.WorldPosition,
                               segmentT
                               );
            }
            return Vector3.Zero;
        }


        private Vector3 Bezier1(Vector3 a, Vector3 b, float t)
        {
            return Vector3.Lerp(a, b, t);
        }

        private Vector3 Bezier2(Vector3 a, Vector3 b, Vector3 c, float t)
        {
            return Vector3.Lerp(Bezier1(a, b, t), Bezier1(b, c, t), t);
        }

        private Vector3 Bezier3(Vector3 a, Vector3 b, Vector3 c, Vector3 d, float t)
        {
            return Vector3.Lerp(Bezier2(a, b, c, t), Bezier2(b, c, d, t), t);
        }
    }
}
