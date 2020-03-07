using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo
{
    class Shader : IDisposable
    {
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

            GL.CompileShader(vertexShader);

            string infoLogVert = GL.GetShaderInfoLog(vertexShader);
            if (infoLogVert != string.Empty) throw new ArgumentException(infoLogVert);

            GL.CompileShader(fragmentShader);

            string infoLogFrag = GL.GetShaderInfoLog(fragmentShader);
            if (infoLogFrag != string.Empty) throw new ArgumentException(infoLogFrag);

            _handle = GL.CreateProgram();

            GL.AttachShader(_handle, vertexShader);
            GL.AttachShader(_handle, fragmentShader);

            GL.LinkProgram(_handle);


            GL.DetachShader(_handle, vertexShader);
            GL.DetachShader(_handle, fragmentShader);
            GL.DeleteShader(vertexShader);
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
    }
}
