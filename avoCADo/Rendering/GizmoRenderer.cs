using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;

namespace avoCADo
{
    public class GizmoRenderer : Renderer
    {
        public GizmoRenderer(Shader shader) : base(shader)
        {
            SetBufferData();
        }

        public override IMeshGenerator GetGenerator()
        {
            return null;
        }

        protected override void Draw()
        {
            GL.LineWidth(2.0f);

            GL.Uniform4(_shaderColorLocation, 1.0f, 0.0f, 0.0f, 1.0f);
            GL.DrawElements(PrimitiveType.Lines, 2, DrawElementsType.UnsignedInt, 0);

            GL.Uniform4(_shaderColorLocation, 0.0f, 1.0f, 0.0f, 1.0f);
            GL.DrawElements(PrimitiveType.Lines, 2, DrawElementsType.UnsignedInt, 2 * sizeof(uint));

            GL.Uniform4(_shaderColorLocation, 0.0f, 0.0f, 1.0f, 1.0f);
            GL.DrawElements(PrimitiveType.Lines, 2, DrawElementsType.UnsignedInt, 4 * sizeof(uint));

            GL.Uniform4(_shaderColorLocation, 1.0f, 0.0f, 0.0f, 1.0f);
            GL.LineWidth(1.0f);
        }

        protected override void SetBufferData()
        {
            GL.BindVertexArray(VAO);
            float[] vertices = 
            {
                0.0f, 0.0f, 0.0f,
                1.0f, 0.0f, 0.0f,
                0.0f, 1.0f, 0.0f,
                0.0f, 0.0f, 1.0f,
            };
            uint[] indices = { 0, 1, 0, 2, 0, 3 };
            _indexCount = indices.Length;
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);
        }
    }
}
