using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo
{
    public class BezierC2PatchControlPointManager : BezierC0PatchControlPointManager
    {
        public BezierC2PatchControlPointManager(NodeFactory nodeFactory, BezierPatchGenerator generator, INode parentNode, IUpdateLoop loop) : base(nodeFactory, generator, parentNode, loop)
        {
        }

        protected override int GetHorizontalAbstractCPCount(int horizontalPatches)
        {
            return 3 + horizontalPatches;
        }
        protected override int GetVerticalAbstractCPCount(int verticalPatches)
        {
            return 3 + verticalPatches;
        }

        protected override int GetHorizontalControlPointCount(int horizontalPatches, PatchType type)
        {
            return type == PatchType.Flat ? 3 + horizontalPatches : horizontalPatches;
        }

        protected override int GetVerticalControlPointCount(int verticalPatches, PatchType type)
        {
            return 3 + verticalPatches;
        }
    }
}
