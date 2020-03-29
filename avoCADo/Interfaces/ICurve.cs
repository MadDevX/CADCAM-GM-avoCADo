using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo
{
    public interface ICurve
    {
        int Segments { get; }

        Vector2 ParameterRange { get; }
        Vector3 GetVertex(float t);

        IList<INode> ControlPoints { get; }

        bool HasVirtualControlPoints { get; }
        IList<Vector3> VirtualControlPoints { get; }

        void Refresh();
        //GetTangent
    }
}
