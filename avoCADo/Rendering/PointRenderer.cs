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
    public class PointRenderer : Renderer
    {
        private Color4 _color;

        public PointRenderer(ShaderWrapper shaderWrapper, Color4 color) : base(shaderWrapper)
        {
            _color = color;
            SetBufferData();
        }

        public override IMeshGenerator GetGenerator() => null;

        protected override void Draw(Camera camera, Matrix4 localMatrix, Matrix4 parentMatrix)
        {
            GL.PointSize(RenderConstants.POINT_SIZE);
            _shaderWrapper.SetColor(_color);
            GL.DrawArrays(PrimitiveType.Points, 0, 1);
            _shaderWrapper.SetColor(Color4.White);
            GL.PointSize(1.0f);
        }

        protected override void SetBufferData()
        {
            GL.BindVertexArray(VAO);
            float[] vertices = { 0.0f, 0.0f, 0.0f };
            uint[] indices = { 0 };
            _indexCount = indices.Length;
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);
        }
    }
}
