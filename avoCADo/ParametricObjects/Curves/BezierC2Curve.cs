using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo.ParametricObjects.Curves
{
    class BezierC2Curve : ICurve
    {
        public Vector2 ParameterRange => new Vector2(0.0f, Segments);

        private int Segments => (ControlPoints.Count + 1) / 3;

        public IList<INode> ControlPoints { get; }

        public IList<Vector3> ControlPointsPositions => ControlPoints.Select((x) => x.Transform.WorldPosition).ToList(); //wrong at so many levels

        private IList<Vector3> _bernsteinPoints = new List<Vector3>();

        public BezierC2Curve(IList<INode> controlPointsSource)
        {
            ControlPoints = controlPointsSource;
        }

        public Vector3 GetVertex(float t)
        {
            throw new NotImplementedException();
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
