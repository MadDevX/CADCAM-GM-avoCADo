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
    public class ParametricObjectRenderer : MeshRenderer
    {
        private ShaderWrapper _curveShaderWrapper;
        private TesselationShaderWrapper _tessShaderWraper;

        public ParametricObjectRenderer(TesselationShaderWrapper tessShaderWrapper, ShaderWrapper curveShaderWrapper, ShaderWrapper shaderWrapper, IMeshGenerator meshGenerator) : base(shaderWrapper, meshGenerator)
        {
            _tessShaderWraper = tessShaderWrapper;
            _curveShaderWrapper = curveShaderWrapper;
        }

        protected override void Draw(Camera camera, Matrix4 localMatrix, Matrix4 parentMatrix)
        {
            var currentShader = _shaderWrapper;
            var calls = _meshGenerator.DrawCalls;
            for (int i = 0; i < calls.Count; i++)
            {
                var color = _node.IsSelected ? calls[i].selectedColor : calls[i].defaultColor;
                
                GL.LineWidth(calls[i].size);
                if (calls[i].shaderType == DrawCallShaderType.Default)
                {
                    if (currentShader != _shaderWrapper)
                    {
                        SetShader(_shaderWrapper, camera, localMatrix, parentMatrix);
                        currentShader = _shaderWrapper;
                    }
                    currentShader.SetColor(color);
                    GL.DrawElements(PrimitiveType.Lines, calls[i].elementCount, DrawElementsType.UnsignedInt, calls[i].startIndex * sizeof(uint));
                }
                else if (calls[i].shaderType == DrawCallShaderType.Curve)
                {
                    if (currentShader != _curveShaderWrapper)
                    {
                        SetShader(_curveShaderWrapper, camera, localMatrix, parentMatrix);
                        currentShader = _curveShaderWrapper;
                    }
                    currentShader.SetColor(color);
                    GL.DrawElements(PrimitiveType.LinesAdjacency, calls[i].elementCount, DrawElementsType.UnsignedInt, calls[i].startIndex * sizeof(uint));
                }
                else if (calls[i].shaderType == DrawCallShaderType.Surface)
                {
                    if (currentShader != _tessShaderWraper)
                    {
                        SetShader(_tessShaderWraper, camera, localMatrix, parentMatrix);
                        currentShader = _tessShaderWraper;
                    }
                    currentShader.SetColor(color);
                    _tessShaderWraper.SetTessLevelOuter0(calls[i].tessLevelOuter0);
                    _tessShaderWraper.SetTessLevelOuter1(calls[i].tessLevelOuter1);
                    GL.DrawElements(PrimitiveType.Patches, calls[i].elementCount, DrawElementsType.UnsignedInt, calls[i].startIndex * sizeof(uint));
                }
                currentShader.SetColor(Color4.White);
                GL.LineWidth(1.0f);
            }
            SetShader(_shaderWrapper, camera, localMatrix, parentMatrix);
        }
    }
}
