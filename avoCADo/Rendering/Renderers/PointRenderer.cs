using avoCADo.Constants;
using avoCADo.Utility;
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

        public PointRenderer(ShaderWrapper shaderWrapper, Color4 color, Color4 selectedColor) : base(shaderWrapper, MeshUtility.PointMesh)
        {
            _color = color;
            _selectedColor = selectedColor;//new Color4(_color.R * 0.5f, _color.G * 0.5f, _color.B * 0.5f, _color.A);
        }

        public override IMeshGenerator GetGenerator() => null;

        protected override void Draw(Camera camera, Matrix4 localMatrix, Matrix4 parentMatrix)
        {
            if (_node.IsSelected)
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

        protected override void SetBufferData()
        {
        }
    }
}
