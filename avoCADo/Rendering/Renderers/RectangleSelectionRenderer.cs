using avoCADo.Constants;
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
    public class RectangleSelectionRenderer : SimpleRenderer
    {
        private ScreenSelectionHandler _screenSelectionHandler;
        private GLControl _control;
        public RectangleSelectionRenderer(SimpleShaderWrapper shader, ScreenSelectionHandler screenSelectionHandler, GLControl control) : base(shader)
        {
            _screenSelectionHandler = screenSelectionHandler;
            _control = control;
        }

        protected override void Draw()
        {
            SetBufferData();
            GL.LineWidth(RenderConstants.GIZMO_SIZE);

            _shaderWrapper.SetColor(Color4.GreenYellow);
            GL.DrawElements(PrimitiveType.LineLoop, 4, DrawElementsType.UnsignedInt, 0 * sizeof(uint));

            _shaderWrapper.SetColor(Color4.White);

            GL.LineWidth(1.0f);
        }

        protected override void SetBufferData()
        {
            GL.BindVertexArray(VAO);
            var a = ScreenSelectionHandler.PixelToNDC(_screenSelectionHandler.StartLocation, _control);
            var b = ScreenSelectionHandler.PixelToNDC(_screenSelectionHandler.EndLocation, _control);
            float[] vertices =
            {
                a.X, a.Y, 0.0f,
                b.X, a.Y, 0.0f,
                b.X, b.Y, 0.0f,
                a.X, b.Y, 0.0f,
            };
            uint[] indices = { 0, 1, 2, 3 };
            _indexCount = indices.Length;
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.DynamicDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.DynamicDraw);
        }
    }
}
