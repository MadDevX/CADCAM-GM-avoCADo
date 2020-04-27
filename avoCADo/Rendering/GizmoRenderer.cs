using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace avoCADo
{
    public class GizmoRenderer : Renderer
    {
        public GizmoRenderer(ShaderWrapper shaderWrapper) : base(shaderWrapper)
        {
            SetBufferData();
        }

        public override IMeshGenerator GetGenerator()
        {
            return null;
        }

        protected override void Draw(Camera camera, Matrix4 localMatrix, Matrix4 parentMatrix)
        {
            GL.LineWidth(RenderConstants.GIZMO_SIZE);

            _shaderWrapper.SetColor(Color4.Red);
            GL.DrawElements(PrimitiveType.Lines, 2, DrawElementsType.UnsignedInt, 0);

            _shaderWrapper.SetColor(Color4.Green);
            GL.DrawElements(PrimitiveType.Lines, 2, DrawElementsType.UnsignedInt, 2 * sizeof(uint));

            _shaderWrapper.SetColor(Color4.Blue);
            GL.DrawElements(PrimitiveType.Lines, 2, DrawElementsType.UnsignedInt, 4 * sizeof(uint));

            _shaderWrapper.SetColor(Color4.White);

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
