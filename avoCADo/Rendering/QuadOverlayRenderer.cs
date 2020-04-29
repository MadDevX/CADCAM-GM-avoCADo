using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo
{
    public class QuadOverlayRenderer : IDisposable
    {
        protected int VAO = 0;
        protected int VBO = 0;
        protected BufferShaderWrapper _shaderWrapper;

        protected bool _shouldDispose = false;
        protected int _indexCount;

        private float[] _quadVertices =
        {
            // positions   // texCoords
            -1.0f,  1.0f,  0.0f, 1.0f,
            -1.0f, -1.0f,  0.0f, 0.0f,
             1.0f, -1.0f,  1.0f, 0.0f,

            -1.0f,  1.0f,  0.0f, 1.0f,
             1.0f, -1.0f,  1.0f, 0.0f,
             1.0f,  1.0f,  1.0f, 1.0f
        };

        public QuadOverlayRenderer(BufferShaderWrapper shader)
        {
            _shaderWrapper = shader;
            InitializeGLObjects();
            SetBufferData();
        }

        public virtual void Dispose()
        {
            DisposeData();
        }

        public void Render()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.BindVertexArray(VAO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            _shaderWrapper.Shader.Use();

            GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
        }

        private void InitializeGLObjects()
        {
            InitializeVBO();
            InitializeVAO();
            _shouldDispose = true;
        }

        private void InitializeVAO()
        {
            VAO = GL.GenVertexArray();
            GL.BindVertexArray(VAO);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 0);
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 2 * sizeof(float));
        }

        private void InitializeVBO()
        {
            VBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
        }

        private void SetBufferData()
        {
            GL.BindVertexArray(VAO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, _quadVertices.Length * sizeof(float), _quadVertices, BufferUsageHint.StaticDraw);
        }

        private void DisposeData()
        {
            if (_shouldDispose)
            {
                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
                GL.BindVertexArray(0);
                GL.DeleteBuffer(VBO);
                GL.DeleteVertexArray(VAO);
                _shouldDispose = false;
            }
        }
    }
}
