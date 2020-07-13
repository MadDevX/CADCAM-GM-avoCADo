using avoCADo.Constants;
using OpenTK;
using OpenTK.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo
{
    public static class ParametricSpaceConverter
    {
        private static DummyNode _node = new DummyNode();

        public static void SetupData(ISurface surf, ISurface second, List<ParametricObjectRenderer> rendList, ShaderProvider shaderProvider, bool selfIntersectionQ = false)
        {
            //TODO: handle more curves on surfaces
            foreach (var c in surf.BoundingCurves)
            {
                if (second.BoundingCurves.Contains(c) == false) continue;
                var gen = new RawDataGenerator();
                var rend = new ParametricObjectRenderer(shaderProvider, gen);
                _node.Assign(rend);
                rendList.Add(rend);

                var list = c.Curve.GetParameterList(surf, selfIntersectionQ);
                var uRng = surf.ParameterURange;
                var vRng = surf.ParameterVRange;
                var positions = list.Select(x => new Vector3(
                    (x.X - uRng.X) / ((uRng.Y - uRng.X) * 0.5f) - 1.0f,
                    (x.Y - vRng.X) / ((vRng.Y - vRng.X) * 0.5f) - 1.0f,
                    0.0f)
                ).ToList();
                var indices = CorrectLooping(positions);
                gen.SetData(positions, indices);
                gen.Size = RenderConstants.CURVE_SIZE;
                gen.SelectedColor = Color4.Red;
                gen.DefaultColor = Color4.Red;
                gen.DrawCallShaderType = DrawCallShaderType.Default;
            }
        }

        private static IList<uint> CorrectLooping(IList<Vector3> vects)
        {
            var alts = new Vector3[8];
            var alts2 = new Vector3[8];

            for (int i = vects.Count - 2; i >= 0; i--)
            {
                var pos = vects[i];
                var pos2 = vects[i + 1];
                var len = (pos - pos2).Length;
                FillAlts(alts, pos, true);
                FillAlts(alts2, pos2, false);

                var minDist = len;
                var minIdx = -1;
                for (int j = 0; j < 8; j++)
                {
                    var altDist = (pos - alts2[j]).Length;
                    if (altDist < minDist)
                    {
                        minIdx = j;
                        minDist = altDist;
                    }
                }

                //found better alternative
                if (minIdx != -1)
                {
                    vects.Insert(i + 1, alts2[minIdx]); //first half of "corrected" edge (normal Pos to corrected Pos2)
                    vects.Insert(i + 2, alts[minIdx]); //second half of "corrected" edge (corrected Pos to normal Pos2)
                }
            }

            var indices = GenerateLineIndices(vects.Count);
            RemoveOffNDCLines(vects, indices);
            return indices;
        }

        private static List<uint> GenerateLineIndices(int vertexCount)
        {
            var len = (vertexCount - 1) * 2;
            var indices = new List<uint>(len);
            for (int i = 0; i < len; i++) indices.Add((uint)((i + 1) / 2));
            return indices;
        }

        private static void RemoveOffNDCLines(IList<Vector3> vertices, List<uint> indices)
        {
            for (int i = indices.Count - 2; i >= 0; i -= 2)
            {
                var maxA = MaxAbsXYCoord(vertices[(int)indices[i]]);
                var maxB = MaxAbsXYCoord(vertices[(int)indices[i + 1]]);

                if (maxA > 1.0f && maxB > 1.0f)
                {
                    indices.RemoveAt(i + 1);
                    indices.RemoveAt(i);
                }
            }
        }

        private static void FillAlts(Vector3[] altTable, Vector3 pos, bool negate)
        {
            var mult = negate ? -1.0f : 1.0f;
            altTable[0] = pos + mult * (2.0f * Vector3.UnitX);                        /* posXP   */ //XP - X/Y(axis) P/N(positive/negative) 
            altTable[1] = pos - mult * (2.0f * Vector3.UnitX);                        /* posXN   */
            altTable[2] = pos + mult * (2.0f * Vector3.UnitY);                        /* posYP   */
            altTable[3] = pos - mult * (2.0f * Vector3.UnitY);                        /* posYN   */
            altTable[4] = pos + mult * (2.0f * Vector3.UnitX + 2.0f * Vector3.UnitY); /* posXPYP */
            altTable[5] = pos + mult * (2.0f * Vector3.UnitX - 2.0f * Vector3.UnitY); /* posXPYN */
            altTable[6] = pos - mult * (2.0f * Vector3.UnitX + 2.0f * Vector3.UnitY); /* posXNYP */
            altTable[7] = pos - mult * (2.0f * Vector3.UnitX - 2.0f * Vector3.UnitY); /* posXNYN */
        }

        private static float MaxAbsXYCoord(Vector3 pos)
        {
            return Math.Max(Math.Abs(pos.X), Math.Abs(pos.Y));
        }
    }
}
