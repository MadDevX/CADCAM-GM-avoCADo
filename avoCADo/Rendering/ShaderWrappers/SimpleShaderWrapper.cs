using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo
{
    public class SimpleShaderWrapper : AbstractShaderWrapper
    {
        private int _shaderColorLocation;

        public SimpleShaderWrapper(Shader shader) : base(shader) { }

        protected override void SetUniformLocations()
        {
            _shaderColorLocation = GL.GetUniformLocation(Shader.Handle, "color");
        }

        public void SetColor(Color4 color)
        {
            CheckShaderBinding();
            GL.Uniform4(_shaderColorLocation, color);
        }
    }
}
