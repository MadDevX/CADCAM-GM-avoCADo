using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo
{
    public class ShaderWrapper : SimpleShaderWrapper
    {
        private int _shaderModelMatrixLocation;
        private int _shaderViewLocation;
        private int _shaderProjectionLocation;
        private int _shaderBgColorLocation;
        private int _shaderFilterColorLocation;

        public ShaderWrapper(Shader shader, string name) : base(shader, name)
        {
        }

        protected override void SetUniformLocations()
        {
            base.SetUniformLocations();
            _shaderModelMatrixLocation = GL.GetUniformLocation(Shader.Handle, "model");
            _shaderViewLocation = GL.GetUniformLocation(Shader.Handle, "view");
            _shaderProjectionLocation = GL.GetUniformLocation(Shader.Handle, "projection");
            _shaderBgColorLocation = GL.GetUniformLocation(Shader.Handle, "bgColor");
            _shaderFilterColorLocation = GL.GetUniformLocation(Shader.Handle, "filterColor");
        }


        public void SetModelMatrix(Matrix4 model)
        {
            CheckShaderBinding();
            GL.UniformMatrix4(_shaderModelMatrixLocation, false, ref model);
        }

        public void SetViewMatrix(Matrix4 view)
        {
            CheckShaderBinding();
            GL.UniformMatrix4(_shaderViewLocation, false, ref view);
        }

        public void SetProjectionMatrix(Matrix4 projection)
        {
            CheckShaderBinding();
            GL.UniformMatrix4(_shaderProjectionLocation, false, ref projection);
        }

        public void SetBackgroundColor(Color4 color)
        {
            CheckShaderBinding();
            GL.Uniform4(_shaderBgColorLocation, color);
        }

        public void SetFilterColor(Color4 color)
        {
            CheckShaderBinding();
            GL.Uniform4(_shaderFilterColorLocation, color);
        }
    }
}
