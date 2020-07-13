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

        public MeshRenderer(ShaderWrapper shaderWrapper, IMeshGenerator meshGenerator, VertexLayout.Type type = VertexLayout.Type.Position) : base(shaderWrapper, type)
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

        protected override void Draw(ICamera camera, Matrix4 localMatrix, Matrix4 parentMatrix)
        {
            GL.DrawElements(PrimitiveType.Lines, _mesh.IndexCount, DrawElementsType.UnsignedInt, 0);
        }

        protected override void SetBufferData()
        {
            float[] vertices = _meshGenerator.GetVertices();
            uint[] indices = _meshGenerator.GetIndices();
            _mesh.SetBufferData(vertices, indices, BufferUsageHint.DynamicDraw);
        }
    }
}
