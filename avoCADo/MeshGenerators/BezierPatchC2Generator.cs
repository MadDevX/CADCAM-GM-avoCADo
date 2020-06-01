using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo
{
    public class BezierPatchC2Generator : BezierPatchGenerator
    {
        protected override DrawCallShaderType SurfaceDrawType => DrawCallShaderType.SurfaceDeBoor;

        public BezierPatchC2Generator(IBezierSurface surface, NodeFactory nodeFactory, PatchType patchType, Vector3 position, int horizontalPatches = 1, int verticalPatches = 1, float width = 1, float height = 1) : base(surface, nodeFactory, patchType, position, horizontalPatches, verticalPatches, width, height)
        {
        }

        protected override BezierC0PatchControlPointManager CreateCPManager(NodeFactory nodeFactory, BezierPatchGenerator generator, INode node)
        {
            return new BezierC2PatchControlPointManager(nodeFactory, generator, node);
        }

        protected override int GetPatchOffset(int patch)
        {
            return patch;
        }
    }
}
