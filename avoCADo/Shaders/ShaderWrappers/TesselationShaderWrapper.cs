using OpenTK;
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

        private bool _textured;
        private int _shaderTrimTextureLocation = -1;
        private int _shaderPatchCoordsLocation = -1;
        private int _shaderPatchDimensionsLocation = -1;
        private int _shaderFlipUVLocation = -1;
        private int _shaderFlipTrimLocation = -1;

        public TesselationShaderWrapper(Shader shader, bool textured, string name) : base(shader, name)
        {
            _textured = textured; 
            if (_textured)
            {
                _shaderTrimTextureLocation = GL.GetUniformLocation(Shader.Handle, "trimTexture");
                _shaderPatchCoordsLocation = GL.GetUniformLocation(Shader.Handle, "patchCoords");
                _shaderPatchDimensionsLocation = GL.GetUniformLocation(Shader.Handle, "patchDimensions");
                _shaderFlipUVLocation = GL.GetUniformLocation(Shader.Handle, "flipUV");
                _shaderFlipTrimLocation = GL.GetUniformLocation(Shader.Handle, "flipTrim");
                SetTrimTexture(0);
            }
        }

        protected override void SetUniformLocations()
        {
            base.SetUniformLocations();
            _shaderTessLevelOuter0Location = GL.GetUniformLocation(Shader.Handle, "tessLevelOuter0");
            _shaderTessLevelOuter1Location = GL.GetUniformLocation(Shader.Handle, "tessLevelOuter1");

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

        public void SetPatchCoords(Vector2 coords)
        {
            CheckShaderBinding();
            if (_textured)
            {
                GL.Uniform2(_shaderPatchCoordsLocation, coords);
            }
        }

        public void SetPatchDimensions(Vector2 dims)
        {
            CheckShaderBinding();
            if (_textured)
            {
                GL.Uniform2(_shaderPatchDimensionsLocation, dims);
            }
        }

        public void SetFlipUV(bool flipUV)
        {
            CheckShaderBinding();
            if (_textured)
            {
                GL.Uniform1(_shaderFlipUVLocation, flipUV?1:0);
            }
        }

        public void SetFlipTrim(bool flipTrim)
        {
            CheckShaderBinding();
            if (_textured)
            {
                GL.Uniform1(_shaderFlipTrimLocation, flipTrim ? 1 : 0);
            }
        }
    }
}
