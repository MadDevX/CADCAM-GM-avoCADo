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
    public class ParametricObjectRenderer : GeneratedMeshRenderer
    {
        private Dictionary<DrawCallShaderType, ShaderWrapper> _shadersDict;
        private Dictionary<DrawCallShaderType, PrimitiveType> _primitivesDict;

        public ParametricObjectRenderer(IShaderProvider provider, IMeshGenerator meshGenerator, VertexLayout.Type type = VertexLayout.Type.Position) : base(provider.DefaultShader, meshGenerator, type)
        {
            _shadersDict = DictionaryInitializer.InitializeEnumDictionary<DrawCallShaderType, ShaderWrapper>(provider.DefaultShader, provider.CurveShader, provider.SurfaceShaderBezier, provider.SurfaceShaderDeBoor, provider.SurfaceShaderGregory, provider.DefaultTexturedShader);
            _primitivesDict = DictionaryInitializer.InitializeEnumDictionary<DrawCallShaderType, PrimitiveType>(PrimitiveType.Lines, PrimitiveType.LinesAdjacency, PrimitiveType.Patches, PrimitiveType.Patches, PrimitiveType.Patches, PrimitiveType.Lines);
        }

        protected override void Draw(ICamera camera, Matrix4 localMatrix, Matrix4 parentMatrix)
        {
            var currentShader = _shaderWrapper;
            var calls = _meshGenerator.DrawCalls;
            for (int i = 0; i < calls.Count; i++)
            {
                GL.LineWidth(calls[i].size);
                var color = OwnerNode.IsSelected ? calls[i].selectedColor : calls[i].defaultColor;
                var shaderWrapper = _shadersDict[calls[i].shaderType];
                if(currentShader != shaderWrapper)
                {
                    SetShader(shaderWrapper, camera, localMatrix, parentMatrix);
                    currentShader = shaderWrapper;
                }
                currentShader.SetColor(color);
                if (calls[i].shaderType == DrawCallShaderType.SurfaceBezier || calls[i].shaderType == DrawCallShaderType.SurfaceDeBoor || calls[i].shaderType == DrawCallShaderType.SurfaceGregory)
                {
                    var tess = shaderWrapper as TesselationShaderWrapper;
                    if (tess != null)
                    {
                        GL.PatchParameter(PatchParameterInt.PatchVertices, calls[i].patchCount);
                        tess.SetTessLevelOuter0(calls[i].tessLevelOuter0);
                        tess.SetTessLevelOuter1(calls[i].tessLevelOuter1);
                    }
                }
                if(shaderWrapper is ITrimmableShaderWrapper trimShader)
                {
                    trimShader.SetPatchCoords(calls[i].patchCoords);
                    trimShader.SetPatchDimensions(calls[i].patchDimensions);
                    trimShader.SetFlipUV(calls[i].flipUV);
                    trimShader.SetFlipTrim(calls[i].flipTrim);
                    trimShader.SetTrim(calls[i].trim);
                }
                GL.DrawElements(_primitivesDict[calls[i].shaderType], calls[i].elementCount, DrawElementsType.UnsignedInt, calls[i].startIndex * sizeof(uint));
                currentShader.SetColor(Color4.White);
                GL.LineWidth(1.0f);
            }
            SetShader(_shaderWrapper, camera, localMatrix, parentMatrix);
        }
    }
}
