using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace avoCADo
{
    public abstract class Renderer : IRenderer, IDisposable
    {
        protected int VAO = 0;
        protected int VBO = 0;
        protected int EBO = 0;
        protected Shader _shader;

        protected bool _shouldDispose = false;
        protected int _indexCount;
        protected int _shaderModelMatrixLocation = -1;
        protected int _shaderColorLocation = -1;

        public Renderer(Shader shader)
        {
            _shader = shader;
            _shaderModelMatrixLocation = GL.GetUniformLocation(_shader.Handle, "model");
            _shaderColorLocation = GL.GetUniformLocation(_shader.Handle, "color");
            InitializeGLObjects();
        }

        public virtual void Dispose()
        {
            DisposeData();
        }

        public abstract IMeshGenerator GetGenerator();

        public void Render(Camera camera, Matrix4 localMatrix, Matrix4 parentMatrix)
        {
            _shader.Use();
            GL.BindVertexArray(VAO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
            SetModelMatrix(localMatrix, parentMatrix);
            camera.SetCameraMatrices(_shader.Handle);
            Draw();
        }

        protected abstract void Draw();

        private void SetModelMatrix(Matrix4 localMatrix, Matrix4 parentMatrix)
        {
            var model = localMatrix * parentMatrix;
            GL.UniformMatrix4(_shaderModelMatrixLocation, false, ref model);
        }

        private void InitializeGLObjects()
        {
            InitializeVBO();
            InitializeEBO();
            InitializeVAO();
            _shouldDispose = true;
        }

        private void InitializeVAO()
        {
            VAO = GL.GenVertexArray();
            GL.BindVertexArray(VAO);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
        }

        private void InitializeVBO()
        {
            VBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
        }

        private void InitializeEBO()
        {
            EBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
        }

        /// <summary>
        /// Use at the end of constructor
        /// </summary>
        protected abstract void SetBufferData();

        private void DisposeData()
        {
            if (_shouldDispose)
            {
                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
                GL.BindVertexArray(0);
                GL.DeleteBuffer(VBO);
                GL.DeleteBuffer(EBO);
                GL.DeleteVertexArray(VAO);
                _shouldDispose = false;
            }
        }
    }
}
