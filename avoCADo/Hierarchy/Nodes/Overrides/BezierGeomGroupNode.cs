using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace avoCADo
{
    public class BezierGeomGroupNode : GroupNode<BezierGeneratorGeometry>
    {
        public override GroupNodeType GroupNodeType => GroupNodeType.Attachable;
        public BezierGeomGroupNode(WpfObservableRangeCollection<INode> childrenSource, IRenderer renderer, BezierGeneratorGeometry dependent, string name) : base(childrenSource, renderer, dependent, name)
        {
        }
    }
}
