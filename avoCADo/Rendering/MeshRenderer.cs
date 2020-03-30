using OpenTK;
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
        protected IMeshGenerator _meshGenerator;

        public MeshRenderer(ShaderWrapper shaderWrapper, IMeshGenerator meshGenerator) : base(shaderWrapper)
        {
            _meshGenerator = meshGenerator;
            _meshGenerator.OnParametersChanged += SetBufferData;
            SetBufferData();
        }

        public override void Dispose()
        {
            _meshGenerator.OnParametersChanged -= SetBufferData;
            _meshGenerator.Dispose();
            base.Dispose();
        }

        public override IMeshGenerator GetGenerator()
        {
            return _meshGenerator;
        }

        protected override void Draw(Camera camera, Matrix4 localMatrix, Matrix4 parentMatrix)
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
