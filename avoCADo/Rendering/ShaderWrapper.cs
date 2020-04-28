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
    public class ShaderWrapper : IDisposable
    {
        public Shader Shader { get; }
        private int _shaderModelMatrixLocation;
        private int _shaderColorLocation;
        private int _shaderViewLocation;
        private int _shaderProjectionLocation;
        private int _shaderBgColorLocation;
        private int _shaderFilterColorLocation;

        public ShaderWrapper(Shader shader)
        {
            Shader = shader;
            _shaderModelMatrixLocation = GL.GetUniformLocation(Shader.Handle, "model");
            _shaderColorLocation = GL.GetUniformLocation(Shader.Handle, "color");
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

        public void SetColor(Color4 color)
        {
            CheckShaderBinding();
            GL.Uniform4(_shaderColorLocation, color);
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

        public void Dispose()
        {
            Shader.Dispose();
        }

        private void CheckShaderBinding()
        {
            //TODO: check if it's possible to check currently bound shader program
            Shader.Use();
        }
    }
}
