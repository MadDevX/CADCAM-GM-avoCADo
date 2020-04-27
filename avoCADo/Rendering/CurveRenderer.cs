﻿using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo
{
    public class CurveRenderer : MeshRenderer
    {
        private ShaderWrapper _curveShaderWrapper;

        public CurveRenderer(ShaderWrapper curveShaderWrapper, ShaderWrapper shaderWrapper, IMeshGenerator meshGenerator) : base(shaderWrapper, meshGenerator)
        {
            _curveShaderWrapper = curveShaderWrapper;
        }

        protected override void Draw(Camera camera, Matrix4 localMatrix, Matrix4 parentMatrix)
        {
            var currentShader = _shaderWrapper;
            var calls = _meshGenerator.DrawCalls;
            for (int i = 0; i < calls.Count; i++)
            {
                GL.LineWidth(calls[i].size);
                if (calls[i].shaderType == DrawCallShaderType.Default)
                {
                    if (currentShader != _shaderWrapper)
                    {
                        SetShader(_shaderWrapper, camera, localMatrix, parentMatrix);
                        currentShader = _shaderWrapper;
                    }
                    currentShader.SetColor(calls[i].color);
                    GL.DrawElements(PrimitiveType.Lines, calls[i].elementCount, DrawElementsType.UnsignedInt, calls[i].startIndex * sizeof(uint));
                }
                else if (calls[i].shaderType == DrawCallShaderType.Curve)
                {
                    if (currentShader != _curveShaderWrapper)
                    {
                        SetShader(_curveShaderWrapper, camera, localMatrix, parentMatrix);
                        currentShader = _curveShaderWrapper;
                    }
                    currentShader.SetColor(calls[i].color);
                    GL.DrawElements(PrimitiveType.LinesAdjacency, calls[i].elementCount, DrawElementsType.UnsignedInt, calls[i].startIndex * sizeof(uint));
                }
                currentShader.SetColor(Color4.White);
                GL.LineWidth(1.0f);
            }
            SetShader(_shaderWrapper, camera, localMatrix, parentMatrix);
        }
    }
}
