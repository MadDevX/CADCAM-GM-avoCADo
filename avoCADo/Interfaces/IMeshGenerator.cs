using System;

namespace avoCADo
{
    public interface IMeshGenerator : IDisposable
    {
        event Action OnParametersChanged;
        float[] GetVertices();
        uint[] GetIndices();
    }
}