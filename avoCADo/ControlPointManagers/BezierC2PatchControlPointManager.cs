using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo
{
    public class BezierC2PatchControlPointManager : BezierC0PatchControlPointManager
    {
        public BezierC2PatchControlPointManager(NodeFactory nodeFactory, BezierPatchGenerator generator, INode parentNode) : base(nodeFactory, generator, parentNode)
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

        protected override int GetHorizontalControlPointCount(int horizontalPatches, WrapMode type)
        {
            return type == WrapMode.Column ? horizontalPatches : 3 + horizontalPatches;
        }

        protected override int GetVerticalControlPointCount(int verticalPatches, WrapMode type)
        {
            return type == WrapMode.Row ? verticalPatches : 3 + verticalPatches;
        }
    }
}
