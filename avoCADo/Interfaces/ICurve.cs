using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo
{
    public interface ICurve
    {
        Vector2 ParameterRange { get; }
        Vector3 GetVertex(float t);

        IList<INode> ControlPoints { get; }

        IList<Vector3> ControlPointsPositions { get; }
        //GetTangent
    }
}
