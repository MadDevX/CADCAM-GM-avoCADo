using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo
{
    public class LineRenderer : MeshRenderer
    {
        public LineRenderer(ShaderWrapper shaderWrapper, IMeshGenerator meshGenerator) : base(shaderWrapper, meshGenerator)
        {
        }

        protected override void Draw(Camera camera, Matrix4 localMatrix, Matrix4 parentMatrix)
        {
            var calls = _meshGenerator.DrawCalls;
            for (int i = 0; i < calls.Count; i++)
            {
                GL.DrawElements(PrimitiveType.Lines, calls[i].elementCount, DrawElementsType.UnsignedInt, calls[i].startIndex);
            }
        }
    }
}
