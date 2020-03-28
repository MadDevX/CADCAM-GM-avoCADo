using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo
{
    public interface ISurface
    {
        event Action ParametersChanged;
        Vector2 ParameterURange { get; }
        Vector2 ParameterVRange { get; }
        bool ULoop { get; }
        bool VLoop { get; }

        Vector3 GetVertex(float u, float v);

        //GetNormal/Binormal
    }
}
