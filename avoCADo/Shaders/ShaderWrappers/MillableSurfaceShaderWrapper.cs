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
        private int _shaderTextureLocation;
        private int _shaderCameraPosLocation;

        public MillableSurfaceShaderWrapper(Shader shader, string name) : base(shader, name)
        {
            _shaderTextureLocation = GL.GetUniformLocation(Shader.Handle, "samp");
            _shaderCameraPosLocation = GL.GetUniformLocation(Shader.Handle, "cameraPos");
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
