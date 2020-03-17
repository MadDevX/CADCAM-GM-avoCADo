using System;

namespace avoCADo
{
    internal interface IMeshGenerator
    {
        event Action OnParametersChanged;
        float[] GetVertices();
        uint[] GetIndices();
    }
}