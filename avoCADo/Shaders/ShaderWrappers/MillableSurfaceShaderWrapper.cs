﻿using OpenTK;
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
        private int _shaderTextureLocation;
        private int _shaderCameraPosLocation;
        private int _shaderWorldWidthLocation;
        private int _shaderWorldHeightLocation;

        public MillableSurfaceShaderWrapper(Shader shader, string name) : base(shader, name)
        {
            _shaderTextureLocation = GL.GetUniformLocation(Shader.Handle, "samp");
            _shaderCameraPosLocation = GL.GetUniformLocation(Shader.Handle, "cameraPos");
            _shaderWorldWidthLocation = GL.GetUniformLocation(Shader.Handle, "worldWidth");
            _shaderWorldHeightLocation = GL.GetUniformLocation(Shader.Handle, "worldHeight");
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

        public void SetTexture(int textureUnit)
        {
            CheckShaderBinding();
            GL.Uniform1(_shaderTextureLocation, textureUnit);
        }

        public void SetCameraPosition(Vector3 position)
        {
            CheckShaderBinding();
            GL.Uniform3(_shaderCameraPosLocation, position);
        }
    }
}
