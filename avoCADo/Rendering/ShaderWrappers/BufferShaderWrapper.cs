using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo
{
    public class BufferShaderWrapper : AbstractShaderWrapper
    {
        private int _bufferTexture1Location;
        private int _bufferTexture2Location;

        public BufferShaderWrapper(Shader shader, string name) : base(shader, name)
        {
            SetTexture1(0);
            SetTexture2(1);
        }



        protected override void SetUniformLocations()
        {
            _bufferTexture1Location = GL.GetUniformLocation(Shader.Handle, "bufferTexture1");
            _bufferTexture2Location = GL.GetUniformLocation(Shader.Handle, "bufferTexture2");
        }

        public void SetTexture1(int textureUnit)
        {
            CheckShaderBinding();
            GL.Uniform1(_bufferTexture1Location, textureUnit);
        }
        public void SetTexture2(int textureUnit)
        {
            CheckShaderBinding();
            GL.Uniform1(_bufferTexture2Location, textureUnit);
        }
    }
}
