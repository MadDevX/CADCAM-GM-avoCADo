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

        public MillableSurfaceShaderWrapper(Shader shader, string name) : base(shader, name)
        {
            GL.GetUniformLocation(Shader.Handle, "samp");
        }


        public void SetTexture(int textureUnit)
        {
            CheckShaderBinding();
            GL.Uniform1(_shaderTextureLocation, textureUnit);
        }
    }
}
