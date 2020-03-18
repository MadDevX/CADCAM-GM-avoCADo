using System;

namespace avoCADo
{
    public interface IMeshGenerator
    {
        event Action OnParametersChanged;
        float[] GetVertices();
        uint[] GetIndices();
    }
}