using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo.Miscellaneous
{
    public class LoopDetector
    {

        public static bool ValidForFilling(INode a, INode b, INode c)
        {
            var common = CommonCPs(a, b, c);
            if (common.HasValue == false) return false;
            var comPts = common.Value;
            var surfA = (a.Renderer.GetGenerator() as BezierPatchGenerator).Surface;
            var surfB = (b.Renderer.GetGenerator() as BezierPatchGenerator).Surface;
            var surfC = (c.Renderer.GetGenerator() as BezierPatchGenerator).Surface;
            if (IsCornerPoint(comPts.ab, surfA) == false ||
                IsCornerPoint(comPts.ab, surfB) == false ||
                IsCornerPoint(comPts.bc, surfB) == false ||
                IsCornerPoint(comPts.bc, surfC) == false ||
                IsCornerPoint(comPts.ca, surfC) == false ||
                IsCornerPoint(comPts.ca, surfA) == false) return false;
            if (GetAffectedBoundaryCurveLength(surfA) != 4 ||
                GetAffectedBoundaryCurveLength(surfB) != 4 ||
                GetAffectedBoundaryCurveLength(surfC) != 4) return false;
            return true;
        }

        private static int GetAffectedBoundaryCurveLength(IBezierSurface surface)
        {
            var cps = surface.ControlPoints;
            var ld = cps[0, 0] as IDependencyCollector;
            var rd = cps[cps.Width - 1, 0] as IDependencyCollector;
            var ru = cps[cps.Width - 1, cps.Height - 1] as IDependencyCollector;
            var lu = cps[0, cps.Height - 1] as IDependencyCollector;
            if (ld.UniqueDependencyCount > 1 && rd.UniqueDependencyCount > 1) return cps.Width;
            if (lu.UniqueDependencyCount > 1 && ru.UniqueDependencyCount > 1) return cps.Width;
            return cps.Height;
        }

        private static bool IsCornerPoint(INode node, IBezierSurface surface)
        {
            var cps = surface.ControlPoints;
            if (cps[0, 0] == node ||
                cps[cps.Width - 1, 0] == node ||
                cps[cps.Width - 1, cps.Height - 1] == node ||
                cps[0, cps.Height - 1] == node) return true;
            return false;
        }

        private static (int u, int v) CoordsOfCorner(INode node, IBezierSurface surface)
        {
            var cps = surface.ControlPoints;
            if (cps[0, 0] == node) return (0, 0);
            if (cps[cps.Width - 1, 0] == node) return (cps.Width - 1, 0);
            if (cps[cps.Width - 1, cps.Height - 1] == node) return (cps.Width - 1, cps.Height - 1);
            if (cps[0, cps.Height - 1] == node) return (0, cps.Height - 1);
            return (-1, -1);
        }

        private static (INode ab, INode bc, INode ca)? CommonCPs(INode a, INode b, INode c)
        {
            var interusedA = GetChildrenWithMultipleUniqueDependencies(a);
            var interusedB = GetChildrenWithMultipleUniqueDependencies(b);
            var interusedC = GetChildrenWithMultipleUniqueDependencies(c);
            var commonAB = GetCommon(interusedA, interusedB);
            var commonBC = GetCommon(interusedB, interusedC);
            var commonCA = GetCommon(interusedC, interusedA);
            if (commonAB != null && commonBC != null && commonCA != null) return (commonAB, commonBC, commonCA);
            else return null;
        }


        private static INode GetCommon(IList<INode> a, IList<INode> b)
        {
            foreach (var ch1 in a)
            {
                foreach (var ch2 in b)
                {
                    if (ch1 == ch2) return ch1;
                }
            }
            return null;
        }

        private static IList<INode> GetChildrenWithMultipleUniqueDependencies(INode node)
        {
            var list = new List<INode>();
            foreach(var child in node.Children)
            {
                if((child as IDependencyCollector).UniqueDependencyCount > 1)
                {
                    list.Add(child);
                }
            }
            return list;
        }
    }
}
