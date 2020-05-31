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

        public BezierPatchC2Generator(IBezierSurface surface, NodeFactory nodeFactory, IUpdateLoop loop, PatchType patchType, Vector3 position, int horizontalPatches = 1, int verticalPatches = 1, float width = 1, float height = 1) : base(surface, nodeFactory, loop, patchType, position, horizontalPatches, verticalPatches, width, height)
        {
        }

        protected override BezierC0PatchControlPointManager CreateCPManager(NodeFactory nodeFactory, BezierPatchGenerator generator, INode node, IUpdateLoop loop)
        {
            return new BezierC2PatchControlPointManager(nodeFactory, generator, node, loop);
        }

        protected override int GetPatchOffset(int patch)
        {
            return patch;
        }
    }
}
