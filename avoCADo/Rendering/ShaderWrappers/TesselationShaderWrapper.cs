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

        public TesselationShaderWrapper(Shader shader, string name) : base(shader, name)
        {
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
    }
}
