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
        private Color4 _selectedColor;
        private SelectionManager _selectionManager;

        public PointRenderer(ShaderWrapper shaderWrapper, Color4 color, Color4 selectedColor) : base(shaderWrapper)
        {
            _color = color;
            _selectedColor = selectedColor;//new Color4(_color.R * 0.5f, _color.G * 0.5f, _color.B * 0.5f, _color.A);
            _selectionManager = NodeSelection.Manager;
            SetBufferData();
        }

        public override IMeshGenerator GetGenerator() => null;

        protected override void Draw(Camera camera, Matrix4 localMatrix, Matrix4 parentMatrix)
        {
            if (IsSelected())
            {
                GL.PointSize(RenderConstants.SELECTED_POINT_SIZE);
                _shaderWrapper.SetColor(_selectedColor);
            }
            else
            {
                GL.PointSize(RenderConstants.POINT_SIZE);
                _shaderWrapper.SetColor(_color);
            }

            GL.DrawArrays(PrimitiveType.Points, 0, 1);
            _shaderWrapper.SetColor(Color4.White);
            GL.PointSize(1.0f);
        }

        private bool IsSelected()
        {
            foreach(var node in _selectionManager.SelectedNodes)
            {
                if (node.Renderer == this) return true;
            }
            return false;
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
