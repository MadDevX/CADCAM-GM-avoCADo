using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo
{
    class BezierPatchGroupNode : GroupNode<BezierPatchGenerator>
    {
        protected override DependencyType ChildrenDependencyType => DependencyType.Strong;

        public BezierPatchGroupNode(ObservableCollection<INode> childrenSource, IRenderer renderer, BezierPatchGenerator dependent, string name) : base(childrenSource, renderer, dependent, name)
        {
        }
    }
}
