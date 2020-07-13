using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace avoCADo
{
    public class TorusShaderWrapper : ShaderWrapper, ITrimmableShaderWrapper
    {
        private bool _textured;
        private int _shaderTrimTextureLocation = -1;
        private int _shaderFlipTrimLocation = -1;
        private int _shaderTrimLocation = -1;

        public TorusShaderWrapper(Shader shader, bool textured, string name) : base(shader, name)
        {
            _textured = textured;
            if (_textured)
            {
                _shaderTrimTextureLocation = GL.GetUniformLocation(Shader.Handle, "trimTexture");
                _shaderTrimLocation = GL.GetUniformLocation(Shader.Handle, "trim");
                _shaderFlipTrimLocation = GL.GetUniformLocation(Shader.Handle, "flipTrim");
                SetTrimTexture(0);
            }
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
            //CheckShaderBinding();
            //if (_textured)
            //{
            //    GL.Uniform2(_shaderPatchCoordsLocation, coords);
            //}
        }

        public void SetPatchDimensions(Vector2 dims)
        {
            //CheckShaderBinding();
            //if (_textured)
            //{
            //    GL.Uniform2(_shaderPatchDimensionsLocation, dims);
            //}
        }

        public void SetFlipUV(bool flipUV)
        {
            //CheckShaderBinding();
            //if (_textured)
            //{
            //    GL.Uniform1(_shaderFlipUVLocation, flipUV ? 1 : 0);
            //}
        }
        public void SetTrim(bool trim)
        {
            CheckShaderBinding();
            if (_textured)
            {
                GL.Uniform1(_shaderTrimLocation, trim ? 1 : 0);
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
