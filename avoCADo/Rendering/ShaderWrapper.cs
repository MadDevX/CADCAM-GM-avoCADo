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

        public ShaderWrapper(Shader shader)
        {
            Shader = shader;
            _shaderModelMatrixLocation = GL.GetUniformLocation(Shader.Handle, "model");
            _shaderColorLocation = GL.GetUniformLocation(Shader.Handle, "color");
            _shaderViewLocation = GL.GetUniformLocation(Shader.Handle, "view");
            _shaderProjectionLocation = GL.GetUniformLocation(Shader.Handle, "projection");
        }

        public void SetModelMatrix(Matrix4 model)
        {
            GL.UniformMatrix4(_shaderModelMatrixLocation, false, ref model);
        }

        public void SetViewMatrix(Matrix4 view)
        {
            GL.UniformMatrix4(_shaderViewLocation, false, ref view);
        }

        public void SetProjectionMatrix(Matrix4 projection)
        {
            GL.UniformMatrix4(_shaderProjectionLocation, false, ref projection);
        }

        public void SetColor(Color4 color)
        {
            GL.Uniform4(_shaderColorLocation, color);
        }

        public void Dispose()
        {
            Shader.Dispose();
        }
    }
}
