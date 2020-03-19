using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo
{
    public class MeshRenderer : Renderer
    {
        private IMeshGenerator _meshGenerator;

        public MeshRenderer(Shader shader, IMeshGenerator meshGenerator) : base(shader)
        {
            _meshGenerator = meshGenerator;
            _meshGenerator.OnParametersChanged += SetBufferData;
            SetBufferData();
        }

        public override void Dispose()
        {
            _meshGenerator.OnParametersChanged -= SetBufferData;
            base.Dispose();
        }

        protected override void Draw()
        {
            GL.DrawElements(PrimitiveType.Lines, _indexCount, DrawElementsType.UnsignedInt, 0);
        }

        protected override void SetBufferData()
        {
            GL.BindVertexArray(VAO);
            float[] vertices = _meshGenerator.GetVertices();
            uint[] indices = _meshGenerator.GetIndices();
            _indexCount = indices.Length;
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);
        }
    }
}
