using OpenTK;
using OpenTK.Graphics;
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
                GL.LineWidth(calls[i].size);
                _shaderWrapper.SetColor(calls[i].color);
                GL.DrawElements(PrimitiveType.Lines, calls[i].elementCount, DrawElementsType.UnsignedInt, calls[i].startIndex * sizeof(uint));
                _shaderWrapper.SetColor(Color4.White);
                GL.LineWidth(1.0f);
            }
        }
    }
}
