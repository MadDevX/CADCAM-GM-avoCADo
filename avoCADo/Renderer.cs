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
        protected int VAO = 0;
        protected int VBO = 0;
        protected int EBO = 0;
        protected Shader _shader;
        protected IMeshGenerator _meshGenerator;

        protected bool _shouldDispose = false;
        protected int _indexCount;

        public Renderer(Shader shader, IMeshGenerator generator)
        {
            _shader = shader;
            _meshGenerator = generator;
            _meshGenerator.OnParametersChanged += UpdateData;
            InitializeData();
        }

        public void Dispose()
        {
            _meshGenerator.OnParametersChanged -= UpdateData;
            DisposeData();
        }

        public Matrix4 GetLocalModelMatrix(Transform transform)
        {
            Matrix4.CreateTranslation(ref transform.position, out Matrix4 trans);
            Matrix4.CreateScale(ref transform.scale, out Matrix4 scale);
            Matrix4.CreateFromQuaternion(ref transform.rotation, out Matrix4 rot);
            return scale * rot * trans;
        }

        public void Render(Transform transform, Camera camera)
        {
            _shader.Use();
            GL.BindVertexArray(VAO);
            SetModelMatrix(transform);
            camera.SetCameraMatrices(_shader.Handle);
            GL.DrawElements(PrimitiveType.Lines, _indexCount, DrawElementsType.UnsignedInt, 0);
        }

        public void Render(Transform transform, Camera camera, Matrix4 parentMatrix)
        {
            _shader.Use();
            GL.BindVertexArray(VAO);
            SetModelMatrix(transform, parentMatrix);
            camera.SetCameraMatrices(_shader.Handle);
            GL.DrawElements(PrimitiveType.Lines, _indexCount, DrawElementsType.UnsignedInt, 0);
        }

        private void SetModelMatrix(Transform transform)
        {
            int location = GL.GetUniformLocation(_shader.Handle, "modelMatrix");
            var model = GetLocalModelMatrix(transform);
            GL.UniformMatrix4(location, false, ref model);
        }

        private void SetModelMatrix(Transform transform, Matrix4 parentMatrix)
        {
            int location = GL.GetUniformLocation(_shader.Handle, "modelMatrix");
            var model = GetLocalModelMatrix(transform) * parentMatrix;
            GL.UniformMatrix4(location, false, ref model);
        }

        private void InitializeData()
        {
            VAO = GL.GenVertexArray();

            GL.BindVertexArray(VAO);

            InitializeVBO();
            InitializeEBO();

            _shouldDispose = true;
        }

        private void InitializeVBO()
        {
            float[] vertices = _meshGenerator.GetVertices();
            VBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.DynamicDraw);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
        }

        private void InitializeEBO()
        {
            uint[] indices = _meshGenerator.GetIndices();
            _indexCount = indices.Length;
            EBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.DynamicDraw);

        }

        private void UpdateData()
        {
            GL.BindVertexArray(VAO);
            float[] vertices = _meshGenerator.GetVertices();
            uint[] indices = _meshGenerator.GetIndices();
            _indexCount = indices.Length;
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.DynamicDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.DynamicDraw);
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
