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
            _handle = GL.CreateProgram();
            vertexShader = AttachShaderFromSource(_handle, vertexPath, ShaderType.VertexShader);
            fragmentShader = AttachShaderFromSource(_handle, fragmentPath, ShaderType.FragmentShader);
            GL.LinkProgram(_handle);
            DisposeShader(_handle, vertexShader);
            DisposeShader(_handle, fragmentShader);
        }

        public Shader(string vertexPath, string geometryPath, string fragmentPath)
        {
            int vertexShader, fragmentShader, geometryShader;

            _handle = GL.CreateProgram();
            vertexShader = AttachShaderFromSource(_handle, vertexPath, ShaderType.VertexShader);
            geometryShader = AttachShaderFromSource(_handle, geometryPath, ShaderType.GeometryShader);
            fragmentShader = AttachShaderFromSource(_handle, fragmentPath, ShaderType.FragmentShader);
            GL.LinkProgram(_handle);
            DisposeShader(_handle, vertexShader);
            DisposeShader(_handle, geometryShader);
            DisposeShader(_handle, fragmentShader);
        }

        public Shader(string vertexPath, string tessellationControlPath, string tessellationEvaluationPath, string fragmentPath)
        {
            int vertexShader, fragmentShader, tessCtrlShader, tessEvShader;

            _handle = GL.CreateProgram();
            vertexShader = AttachShaderFromSource(_handle, vertexPath, ShaderType.VertexShader);
            tessCtrlShader = AttachShaderFromSource(_handle, tessellationControlPath, ShaderType.TessControlShader);
            tessEvShader = AttachShaderFromSource(_handle, tessellationEvaluationPath, ShaderType.TessEvaluationShader);
            fragmentShader = AttachShaderFromSource(_handle, fragmentPath, ShaderType.FragmentShader);
            GL.LinkProgram(_handle);
            DisposeShader(_handle, vertexShader);
            DisposeShader(_handle, tessCtrlShader);
            DisposeShader(_handle, tessEvShader);
            DisposeShader(_handle, fragmentShader);
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


        private int AttachShaderFromSource(int programHandle, string shaderPath, ShaderType type)
        {
            string shaderSource = ReadSourceCode(shaderPath);
            int shader = GL.CreateShader(type);
            GL.ShaderSource(shader, shaderSource);
            CompileShader(shader);
            GL.AttachShader(programHandle, shader);
            return shader;
        }

        private void DisposeShader(int programHandle, int shaderHandle)
        {
            GL.DetachShader(programHandle, shaderHandle);
            GL.DeleteShader(shaderHandle);
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
