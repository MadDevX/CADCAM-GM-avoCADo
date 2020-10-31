using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo.Shaders.ShaderWrappers
{
    public class MillableSurfaceShaderWrapper : ShaderWrapper
    {
        private int _shaderHeightmapTextureLocation;
        private int _shaderColorTextureLocation;
        private int _shaderCameraPosLocation;
        private int _shaderWorldWidthLocation;
        private int _shaderWorldHeightLocation;
        private int _shaderUseTextureLocation;

        public MillableSurfaceShaderWrapper(Shader shader, string name) : base(shader, name)
        {
            _shaderHeightmapTextureLocation = GL.GetUniformLocation(Shader.Handle, "samp");
            _shaderColorTextureLocation = GL.GetUniformLocation(Shader.Handle, "colorTexture");
            _shaderCameraPosLocation = GL.GetUniformLocation(Shader.Handle, "cameraPos");
            _shaderWorldWidthLocation = GL.GetUniformLocation(Shader.Handle, "worldWidth");
            _shaderWorldHeightLocation = GL.GetUniformLocation(Shader.Handle, "worldHeight");
            _shaderUseTextureLocation = GL.GetUniformLocation(Shader.Handle, "useTexture");
        }

        public void SetUseTexture(int useTexture)
        {
            CheckShaderBinding();
            GL.Uniform1(_shaderUseTextureLocation, useTexture);
        }

        public void SetWorldWidth(float worldWidth)
        {
            CheckShaderBinding();
            GL.Uniform1(_shaderWorldWidthLocation, worldWidth);
        }

        public void SetWorldHeight(float worldHeight)
        {
            CheckShaderBinding();
            GL.Uniform1(_shaderWorldHeightLocation, worldHeight);
        }

        public void SetColorTexture(int textureUnit)
        {
            CheckShaderBinding();
            GL.Uniform1(_shaderColorTextureLocation, textureUnit);
        }

        public void SetHeightmapTexture(int textureUnit)
        {
            CheckShaderBinding();
            GL.Uniform1(_shaderHeightmapTextureLocation, textureUnit);
        }

        public void SetCameraPosition(Vector3 position)
        {
            CheckShaderBinding();
            GL.Uniform3(_shaderCameraPosLocation, position);
        }
    }
}
