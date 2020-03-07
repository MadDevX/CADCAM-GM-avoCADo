using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace avoCADo
{
    class Renderer : IDisposable
    {
        private int VAO = 0;
        private int VBO = 0;
        private Shader _shader;

        private bool _shouldDispose = false;

        public Renderer(Shader shader)
        {
            _shader = shader;
            InitializeData();
        }

        public Renderer(Shader shader, int VAO)
        {
            _shader = shader;
            this.VAO = VAO;
        }

        public void Dispose()
        {
            DisposeData();
        }

        public void Render(Transform transform)
        {
            _shader.Use();
            GL.BindVertexArray(VAO);
            SetModelMatrix(transform);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 3);
        }

        private void SetModelMatrix(Transform transform)
        {
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            GL.Scale(transform.scale);
            GL.Rotate(transform.rotation.Z, Vector3.UnitZ);
            GL.Rotate(transform.rotation.Y, Vector3.UnitY);
            GL.Rotate(transform.rotation.X, Vector3.UnitX);
            GL.Translate(transform.position);
        }

        private void InitializeData()
        {
            VAO = GL.GenVertexArray();

            GL.BindVertexArray(VAO);

            InitializeVBO();

            _shouldDispose = true;
        }

        private void InitializeVBO()
        {
            float[] vertices =
            {
            -0.5f, -0.5f, 0.0f,
             0.5f, -0.5f, 0.0f,
             0.0f,  0.5f, 0.0f
            };

            VBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
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
