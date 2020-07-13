using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo
{
    public class TesselationShaderWrapper : ShaderWrapper
    {
        private int _shaderTessLevelOuter0Location;
        private int _shaderTessLevelOuter1Location;
        private int _shaderTrimTextureLocation;
        private bool _textured;
        public TesselationShaderWrapper(Shader shader, bool textured, string name) : base(shader, name)
        {
            _textured = textured;
        }

        protected override void SetUniformLocations()
        {
            base.SetUniformLocations();
            _shaderTessLevelOuter0Location = GL.GetUniformLocation(Shader.Handle, "tessLevelOuter0");
            _shaderTessLevelOuter1Location = GL.GetUniformLocation(Shader.Handle, "tessLevelOuter1");
            if(_textured)
            {
                _shaderTrimTextureLocation = GL.GetUniformLocation(Shader.Handle, "trimTexture");
                SetTrimTexture(0);
            }
        }

        public void SetTessLevelOuter0(int tessLevel)
        {
            CheckShaderBinding();
            GL.Uniform1(_shaderTessLevelOuter0Location, tessLevel);
        }

        public void SetTessLevelOuter1(int tessLevel)
        {
            CheckShaderBinding();
            GL.Uniform1(_shaderTessLevelOuter1Location, tessLevel);
        }

        public void SetTrimTexture(int textureUnit)
        {
            CheckShaderBinding();
            if (_textured)
            {
                GL.Uniform1(_shaderTrimTextureLocation, textureUnit);
            }
        }
    }
}
