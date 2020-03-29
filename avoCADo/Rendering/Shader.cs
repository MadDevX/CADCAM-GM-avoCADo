using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo
{
    public class Shader : IDisposable
    {
        public int Handle => _handle;

        private int _handle;
        private bool _disposedValue = false;

        public Shader(string vertexPath, string fragmentPath)
        {
            int vertexShader, fragmentShader;

            string vertexShaderSource = ReadSourceCode(vertexPath);
            string fragmentShaderSource = ReadSourceCode(fragmentPath);

            vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, vertexShaderSource);

            fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, fragmentShaderSource);


            CompileShader(vertexShader);
            CompileShader(fragmentShader);

            _handle = GL.CreateProgram();

            GL.AttachShader(_handle, vertexShader);
            GL.AttachShader(_handle, fragmentShader);

            GL.LinkProgram(_handle);


            GL.DetachShader(_handle, vertexShader);
            GL.DetachShader(_handle, fragmentShader);

            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);
        }

        public Shader(string vertexPath, string geometryPath, string fragmentPath)
        {
            int vertexShader, fragmentShader, geometryShader;

            string vertexShaderSource = ReadSourceCode(vertexPath);
            string fragmentShaderSource = ReadSourceCode(fragmentPath);
            string geometryShaderSource = ReadSourceCode(geometryPath);

            vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, vertexShaderSource);

            geometryShader = GL.CreateShader(ShaderType.GeometryShader);
            GL.ShaderSource(geometryShader, geometryShaderSource);

            fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, fragmentShaderSource);

            CompileShader(vertexShader);
            CompileShader(geometryShader);
            CompileShader(fragmentShader);

            _handle = GL.CreateProgram();

            GL.AttachShader(_handle, vertexShader);
            GL.AttachShader(_handle, geometryShader);
            GL.AttachShader(_handle, fragmentShader);

            GL.LinkProgram(_handle);

            GL.DetachShader(_handle, vertexShader);
            GL.DetachShader(_handle, geometryShader);
            GL.DetachShader(_handle, fragmentShader);

            GL.DeleteShader(vertexShader);
            GL.DeleteShader(geometryShader);
            GL.DeleteShader(fragmentShader);
        }

        public void Use()
        {
            GL.UseProgram(_handle);
        }

        public void Dispose()
        {
            if(_disposedValue == false)
            {
                GL.DeleteProgram(_handle);
                _disposedValue = true;
            }
        }

        private string ReadSourceCode(string path)
        {
            using (var reader = new StreamReader(path, Encoding.UTF8))
            {
                return reader.ReadToEnd();
            }
        }

        private void CompileShader(int handle)
        {
            GL.CompileShader(handle);

            string infoLog = GL.GetShaderInfoLog(handle);
            if (infoLog != string.Empty) throw new ArgumentException(infoLog);
        }
    }
}
