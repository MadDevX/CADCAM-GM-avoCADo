using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo
{
    public class PatchRenderer : MeshRenderer
    {
        private static float[] _tessOuter = { 4.0f, 64.0f};
        private TesselationShaderWrapper _tessShaderWrapper;
        public PatchRenderer(TesselationShaderWrapper shaderWrapper, IMeshGenerator meshGenerator) : base(shaderWrapper, meshGenerator)
        {
            _tessShaderWrapper = shaderWrapper;
        }

        protected override void Draw(Camera camera, Matrix4 localMatrix, Matrix4 parentMatrix)
        {
            var calls = _meshGenerator.DrawCalls;
            for (int i = 0; i < calls.Count; i++)
            {
                _tessShaderWrapper.SetTessLevelOuter0(4);
                _tessShaderWrapper.SetTessLevelOuter1(64);
                GL.LineWidth(calls[i].size);
                _shaderWrapper.SetColor(calls[i].color);
                GL.DrawElements(PrimitiveType.Patches, calls[i].elementCount, DrawElementsType.UnsignedInt, calls[i].startIndex * sizeof(uint));
                _shaderWrapper.SetColor(Color4.White);
                GL.LineWidth(1.0f);
            }
        }
    }
}
