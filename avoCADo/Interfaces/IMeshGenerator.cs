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

        IList<DrawCall> DrawCalls { get; }
    }

    public struct DrawCall
    {
        public int startIndex;
        public int elementCount;
        public DrawCallShaderType shaderType;

        public DrawCall(int startIndex, int elementCount, DrawCallShaderType shaderType)
        {
            this.startIndex = startIndex;
            this.elementCount = elementCount;
            this.shaderType = shaderType;
        }
    }

    public enum DrawCallShaderType
    {
        Default,
        Curve
    }
}