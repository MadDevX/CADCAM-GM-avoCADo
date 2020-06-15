using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo.Constants
{
    public static class DefaultNodeNames
    {
        public static string Point { get; } = "Point";
        public static string Torus { get; } = "Torus";
        public static string BezierCurveC0 { get; } = "BezierCurve";
        public static string BezierCurveC2 {get; } = "BSplineCurve";
        public static string InterpolatingCurve { get; } = "InterpolatingC2Curve";
        public static string BezierPatchC0 { get; } = "BezierPatch";
        public static string BezierPatchC2 { get; } = "BSplinePatch";
        public static string GregoryPatch { get; } = "GregoryPatch";
    }
}
