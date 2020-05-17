using OpenTK.Graphics;
using System;
using System.Collections.Generic;

namespace avoCADo
{

    //ICurve/ISurface - GetVertexAtParams(u,v)
    public interface IMeshGenerator : IDisposable
    {
        event Action OnParametersChanged;
        float[] GetVertices();
        uint[] GetIndices();

        void RefreshDataPreRender();

        IList<DrawCall> DrawCalls { get; }
    }

    public struct DrawCall
    {
        public int startIndex;
        public int elementCount;
        public DrawCallShaderType shaderType;
        public float size;
        public Color4 color;

        public DrawCall(int startIndex, int elementCount, DrawCallShaderType shaderType, float size, Color4 color)
        {
            this.startIndex = startIndex;
            this.elementCount = elementCount;
            this.shaderType = shaderType;
            this.size = size;
            this.color = color;
        }

        public DrawCall(int startIndex, int elementCount, DrawCallShaderType shaderType, float size)
        {
            this.startIndex = startIndex;
            this.elementCount = elementCount;
            this.shaderType = shaderType;
            this.size = size;
            this.color = Color4.White;
        }
    }

    public enum DrawCallShaderType
    {
        Default,
        Curve,
        Surface
    }
}