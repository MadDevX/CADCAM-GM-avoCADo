using avoCADo.Constants;
using OpenTK;
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
        public Color4 defaultColor;
        public Color4 selectedColor;
        public int patchCount;
        public int tessLevelOuter0;
        public int tessLevelOuter1;
        public Vector2 patchCoords;
        public Vector2 patchDimensions;
        public bool flipUV;

        public DrawCall(int startIndex, int elementCount, DrawCallShaderType shaderType, float size, Color4 defaultColor, Color4 selectedColor, int patchCount = 16, int outerTess0 = 0, int outerTess1 = 0)
        {
            this.startIndex = startIndex;
            this.elementCount = elementCount;
            this.shaderType = shaderType;
            this.size = size;
            this.defaultColor = defaultColor;
            this.selectedColor = selectedColor;
            this.patchCount = patchCount;
            this.tessLevelOuter0 = outerTess0;
            this.tessLevelOuter1 = outerTess1;
            this.patchCoords = Vector2.Zero;
            this.patchDimensions = Vector2.Zero;
            this.flipUV = false;
        }

        public DrawCall(int startIndex, int elementCount, DrawCallShaderType shaderType, float size, int patchCount = 16, int outerTess0 = 0, int outerTess1 = 0) 
            : this(startIndex, elementCount, shaderType, size, RenderConstants.PARAMETRIC_OBJECT_DEFAULT_COLOR, RenderConstants.PARAMETRIC_OBJECT_SELECTED_COLOR, patchCount, outerTess0, outerTess1)
        {}

        public DrawCall(int startIndex, int elementCount, DrawCallShaderType shaderType, float size, Color4 defaultColor, Color4 selectedColor, int patchCount, int outerTess0, int outerTess1, Vector2 patchCoords, Vector2 patchDims, bool flipUV)
            : this(startIndex, elementCount, shaderType, size, defaultColor, selectedColor, patchCount, outerTess0, outerTess1)
        {
            this.patchCoords = patchCoords;
            this.patchDimensions = patchDims;
            this.flipUV = flipUV;
        }

        public DrawCall(int startIndex, int elementCount, DrawCallShaderType shaderType, float size, int patchCount, int outerTess0, int outerTess1, Vector2 patchCoords, Vector2 patchDims, bool flipUV)
            : this(startIndex, elementCount, shaderType, size, RenderConstants.PARAMETRIC_OBJECT_DEFAULT_COLOR, RenderConstants.PARAMETRIC_OBJECT_SELECTED_COLOR, patchCount, outerTess0, outerTess1, patchCoords, patchDims, flipUV)
        {}
    }

    public enum DrawCallShaderType
    {
        Default,
        Curve,
        SurfaceBezier,
        SurfaceDeBoor,
        SurfaceGregory
    }
}