using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo.ParametricObjects.Curves
{
    class BezierC0Curve : ICurve
    {
        public Vector2 ParameterRange { get; } = new Vector2(0.0f, 1.0f);

        public Vector3 GetVertex(float t)
        {
            return Vector3.Zero;
        }
    }
}
