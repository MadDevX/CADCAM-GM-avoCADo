using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace avoCADo
{
    public class BezierGroupNode : GroupNode<BezierGenerator>
    {
        public override GroupNodeType GroupNodeType => GroupNodeType.Attachable;
        public BezierGroupNode(WpfObservableRangeCollection<INode> childrenSource, IRenderer renderer, BezierGenerator dependent, string name) : base(childrenSource, renderer, dependent, name)
        {
        }
    }
}
