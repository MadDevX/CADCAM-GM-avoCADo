using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo.Miscellaneous
{
    public class LoopDetector
    {
        enum DirCase
        {
            Hor,
            HorR,
            Ver,
            VerR
        }
        enum EdgeCase
        {
            /// <summary>
            /// Bottom
            /// </summary>
            B,
            /// <summary>
            /// Bottom Reverse
            /// </summary>
            BR,
            /// <summary>
            /// Top
            /// </summary>
            T,
            /// <summary>
            /// Top Reverse
            /// </summary>
            TR,
            /// <summary>
            /// Left
            /// </summary>
            L,
            /// <summary>
            /// Left Reverse
            /// </summary>
            LR,
            /// <summary>
            /// Right
            /// </summary>
            R,
            /// <summary>
            /// Right Reverse
            /// </summary>
            RR
        }

        public static IList<CoordList<INode>> GetLoopedCoords(INode a, INode b, INode c)
        {
            var coordsLists = new List<CoordList<INode>>();
            var surfA = (a.Renderer.GetGenerator() as BezierPatchGenerator).Surface;
            var surfB = (b.Renderer.GetGenerator() as BezierPatchGenerator).Surface;
            var surfC = (c.Renderer.GetGenerator() as BezierPatchGenerator).Surface;
            var common = CommonCPs(surfA, surfB, surfC);
            if (common.HasValue == false) throw new InvalidOperationException("Provided surfaces do not create triangular loop");
            var comPts = common.Value;
            var caseA = GetEdgeCase(comPts.ca, comPts.ab, surfA);
            var caseB = GetEdgeCase(comPts.ab, comPts.bc, surfB);
            var caseC = GetEdgeCase(comPts.bc, comPts.ca, surfC);
            coordsLists.Add(RemapCoords(surfA.ControlPoints, caseA));
            coordsLists.Add(RemapCoords(surfB.ControlPoints, caseB));
            coordsLists.Add(RemapCoords(surfC.ControlPoints, caseC));
            return coordsLists;
        }

        private static CoordList<T> RemapCoords<T>(CoordList<T> coordList, EdgeCase edgeCase)
        {
            var remapped = new CoordList<T>();

            switch (edgeCase)
            {
                case EdgeCase.B:
                case EdgeCase.BR:
                case EdgeCase.T:
                case EdgeCase.TR:
                    remapped.ResetSize(coordList.Width, coordList.Height);
                    break;
                case EdgeCase.L:
                case EdgeCase.LR:
                case EdgeCase.R:
                case EdgeCase.RR:
                    remapped.ResetSize(coordList.Height, coordList.Width);
                    break;
            }

            switch (edgeCase)
            {
                case EdgeCase.B:
                    for (int u = 0; u < coordList.Width; u++)
                        for (int v = 0; v < coordList.Height; v++)
                            remapped[u, v] = coordList[u, v];
                    break;
                case EdgeCase.BR:
                    for (int u = 0; u < coordList.Width; u++)
                        for (int v = 0; v < coordList.Height; v++)
                            remapped[coordList.Width - 1 - u, v] = coordList[u, v];
                    break;
                case EdgeCase.T:
                    for (int u = 0; u < coordList.Width; u++)
                        for (int v = 0; v < coordList.Height; v++)
                            remapped[u, coordList.Height - 1 - v] = coordList[u, v];
                    break;
                case EdgeCase.TR:
                    for (int u = 0; u < coordList.Width; u++)
                        for (int v = 0; v < coordList.Height; v++)
                            remapped[coordList.Width - 1 - u, coordList.Height - 1 - v] = coordList[u, v];
                    break;
                case EdgeCase.L:
                    for (int u = 0; u < coordList.Width; u++)
                        for (int v = 0; v < coordList.Height; v++)
                            remapped[v, u] = coordList[u, v];
                    break;
                case EdgeCase.LR:
                    for (int u = 0; u < coordList.Width; u++)
                        for (int v = 0; v < coordList.Height; v++)
                            remapped[coordList.Height - 1 - v, u] = coordList[u, v];
                    break;
                case EdgeCase.R:
                    for (int u = 0; u < coordList.Width; u++)
                        for (int v = 0; v < coordList.Height; v++)
                            remapped[v, coordList.Width - 1 - u] = coordList[u, v];
                    break;
                case EdgeCase.RR:
                    for (int u = 0; u < coordList.Width; u++)
                        for (int v = 0; v < coordList.Height; v++)
                            remapped[coordList.Height - 1 - v, coordList.Width - 1 - u] = coordList[u, v];
                    break;
            }

            return remapped;
        }

        private static EdgeCase GetEdgeCase(INode first, INode second, IBezierSurface surface)
        {
            var coordsA = CoordsOfCorner(first, surface);
            var coordsB = CoordsOfCorner(second, surface);
            if (coordsA.u == -1 || coordsB.u == -1) throw new InvalidOperationException("Point does not belong to provided surface");
            var dirCase = GetDirCase(first, second, surface);
            
            switch (dirCase)
            {
                case DirCase.Hor:
                    return coordsA.v == 0 ? EdgeCase.B : EdgeCase.T;
                case DirCase.HorR:
                    return coordsA.v == 0 ? EdgeCase.BR : EdgeCase.TR;
                case DirCase.Ver:
                    return coordsA.u == 0 ? EdgeCase.L : EdgeCase.R;
                case DirCase.VerR:
                    return coordsA.u == 0 ? EdgeCase.LR : EdgeCase.RR;
                default:
                    throw new InvalidOperationException("Unexpected EdgeCase occurred");
            }
        }

        private static DirCase GetDirCase(INode first, INode second, IBezierSurface surface)
        {
            var coordsA = CoordsOfCorner(first, surface);
            var coordsB = CoordsOfCorner(second, surface);
            if (coordsA.u == -1 || coordsB.u == -1) throw new InvalidOperationException("Point does not belong to provided surface");
            if (coordsA.v == coordsB.v)
            {
                if (coordsA.u <= coordsB.u) return DirCase.Hor;
                else return DirCase.HorR;
            }
            else
            {
                if (coordsA.v <= coordsB.v) return DirCase.Ver;
                else return DirCase.VerR;
            }
        }


        public static bool ValidForFilling(INode a, INode b, INode c)
        {
            var surfA = (a.Renderer.GetGenerator() as BezierPatchGenerator).Surface;
            var surfB = (b.Renderer.GetGenerator() as BezierPatchGenerator).Surface;
            var surfC = (c.Renderer.GetGenerator() as BezierPatchGenerator).Surface;
            var common = CommonCPs(surfA, surfB, surfC);
            if (common.HasValue == false) return false;
            var comPts = common.Value;
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
            var l = 0;
            var r = cps.Width - 1;
            var d = 0;
            var u = cps.Height - 1;
            if (cps[l, d] == node ||
                cps[r, d] == node ||
                cps[r, u] == node ||
                cps[l, u] == node) return true;
            return false;
        }

        private static (int u, int v) CoordsOfCorner(INode node, IBezierSurface surface)
        {
            var cps = surface.ControlPoints;
            var l = 0;
            var r = cps.Width - 1;
            var d = 0;
            var u = cps.Height - 1;
            if (cps[l, d] == node) return (l, d);
            if (cps[r, d] == node) return (r, d);
            if (cps[r, u] == node) return (r, u);
            if (cps[l, u] == node) return (l, u);
            return (-1, -1);
        }

        private static (INode ab, INode bc, INode ca)? CommonCPs(IBezierSurface a, IBezierSurface b, IBezierSurface c)
        {
            var interusedA = GetCornersWithMultipleUniqueDependencies(a);
            var interusedB = GetCornersWithMultipleUniqueDependencies(b);
            var interusedC = GetCornersWithMultipleUniqueDependencies(c);
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

        private static IList<INode> GetCornersWithMultipleUniqueDependencies(IBezierSurface surface)
        {
            var list = new List<INode>();
            var cps = surface.ControlPoints;
            var l = 0;
            var r = cps.Width - 1;
            var d = 0;
            var u = cps.Height - 1;
            if ((cps[l, d] as IDependencyCollector).UniqueDependencyCount > 1) list.Add(cps[l, d]);
            if ((cps[r, d] as IDependencyCollector).UniqueDependencyCount > 1) list.Add(cps[r, d]);
            if ((cps[r, u] as IDependencyCollector).UniqueDependencyCount > 1) list.Add(cps[r, u]);
            if ((cps[l, u] as IDependencyCollector).UniqueDependencyCount > 1) list.Add(cps[l, u]);
            return list;
        }

        //public static void TestRemap()
        //{
        //    var list = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8 };
        //    var c1 = new CoordList<int>(list, 3, 3);

        //    var b = RemapCoords(c1, EdgeCase.B).RawData;
        //    var br = RemapCoords(c1, EdgeCase.BR).RawData;
        //    var t = RemapCoords(c1, EdgeCase.T).RawData;
        //    var tr = RemapCoords(c1, EdgeCase.TR).RawData;
        //    var l = RemapCoords(c1, EdgeCase.L).RawData;
        //    var lr = RemapCoords(c1, EdgeCase.LR).RawData;
        //    var r = RemapCoords(c1, EdgeCase.R).RawData;
        //    var rr = RemapCoords(c1, EdgeCase.RR).RawData;

        //    list.Clear();
        //}
    }
}
