using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace avoCADo
{
    class BezierPatchGroupNode : GroupNode<BezierPatchGenerator>
    {
        public override GroupNodeType GroupNodeType => GroupNodeType.Fixed;
        public override NodeType NodeType => NodeType.Surface;
        public override DependencyType ChildrenDependencyType => DependencyType.Strong;

        public BezierPatchGroupNode(WpfObservableRangeCollection<INode> childrenSource, IRenderer renderer, BezierPatchGenerator dependent, string name) : base(childrenSource, renderer, dependent, name)
        {
        }
    }
}
