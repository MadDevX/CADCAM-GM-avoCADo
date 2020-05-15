using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo
{
    public interface IBezierSurface : ISurface
    {
        int USegments { get; }
        int VSegments { get; }
        CoordList<INode> ControlPoints { get; }
    }
}
