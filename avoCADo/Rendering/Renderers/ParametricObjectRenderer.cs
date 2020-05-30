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

        private Dictionary<DrawCallShaderType, ShaderWrapper> _shadersDict;
        private Dictionary<DrawCallShaderType, PrimitiveType> _primitivesDict;

        public ParametricObjectRenderer(TesselationShaderWrapper tessShaderWrapper, ShaderWrapper curveShaderWrapper, ShaderWrapper shaderWrapper, IMeshGenerator meshGenerator) : base(shaderWrapper, meshGenerator)
        {
            _tessShaderWraper = tessShaderWrapper;
            _curveShaderWrapper = curveShaderWrapper;
            _shadersDict = DictionaryInitializer.InitializeEnumDictionary<DrawCallShaderType, ShaderWrapper>(_shaderWrapper, _curveShaderWrapper, _tessShaderWraper);
            _primitivesDict = DictionaryInitializer.InitializeEnumDictionary<DrawCallShaderType, PrimitiveType>(PrimitiveType.Lines, PrimitiveType.LinesAdjacency, PrimitiveType.Patches);
        }

        protected override void Draw(Camera camera, Matrix4 localMatrix, Matrix4 parentMatrix)
        {
            var currentShader = _shaderWrapper;
            var calls = _meshGenerator.DrawCalls;
            for (int i = 0; i < calls.Count; i++)
            {
                GL.LineWidth(calls[i].size);
                var color = _node.IsSelected ? calls[i].selectedColor : calls[i].defaultColor;
                var shaderWrapper = _shadersDict[calls[i].shaderType];
                if(currentShader != shaderWrapper)
                {
                    SetShader(shaderWrapper, camera, localMatrix, parentMatrix);
                    currentShader = shaderWrapper;
                }
                currentShader.SetColor(color);
                if (calls[i].shaderType == DrawCallShaderType.Surface)
                {
                    _tessShaderWraper.SetTessLevelOuter0(calls[i].tessLevelOuter0);
                    _tessShaderWraper.SetTessLevelOuter1(calls[i].tessLevelOuter1);
                }
                GL.DrawElements(_primitivesDict[calls[i].shaderType], calls[i].elementCount, DrawElementsType.UnsignedInt, calls[i].startIndex * sizeof(uint));
                currentShader.SetColor(Color4.White);
                GL.LineWidth(1.0f);
            }
            SetShader(_shaderWrapper, camera, localMatrix, parentMatrix);
        }
    }
}
