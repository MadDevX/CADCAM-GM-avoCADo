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
        VirtualPoint,
        Scene,
        GregoryPatch,
        IntersectionCurve,
        MillableSurface,
        Cylinder
    }

    public interface IObject
    {
        ObjectType ObjectType { get; set; }
    }
}
