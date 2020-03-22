using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo
{
    public class BezierGroupNode : GroupNode<BezierGenerator>
    {
        public BezierGroupNode(IRenderer renderer, BezierGenerator dependent, string name) : base(renderer, dependent, name)
        {}
    }
}
