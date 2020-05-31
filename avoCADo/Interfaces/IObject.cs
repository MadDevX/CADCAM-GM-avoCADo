using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo
{
    public enum ObjectType
    {
        Point,
        Torus,
        InterpolatingCurve,
        BezierCurveC0,
        BezierCurveC2,
        BezierPatchC0,
        BezierPatchC2,
    }

    public interface IObject
    {
        ObjectType ObjectType { get; set; }
    }
}
