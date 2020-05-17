using avoCADo.Utility;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo
{
    public class Mesh : IDisposable
    {
        private int VAO = 0;
        private int VBO = 0;
        private int EBO = 0;

        public int IndexCount { get; private set; }

        private bool _shouldDispose;

        public Mesh()
        {
            InitializeGLObjects();
        }

        public void Dispose()
        {
            DisposeData();
        }

        public void BindMesh()
        {
            if (MeshUtility.BoundMesh != this)
            {
                GL.BindVertexArray(VAO);
                GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
                MeshUtility.BoundMesh = this;
            }
        }

        public void SetBufferData(float[] vertices, uint[] indices, BufferUsageHint dataType)
        {
            GL.BindVertexArray(VAO);
            IndexCount = indices.Length;
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, dataType);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, dataType);
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
