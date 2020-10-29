using avoCADo.MeshGenerators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace avoCADo
{
    class GregoryPatchGroupNode : GroupNode<GregoryPatchGenerator>
    {
        public override GroupNodeType GroupNodeType => GroupNodeType.Fixed;
        public override DependencyType ChildrenDependencyType => DependencyType.Strong;

        public GregoryPatchGroupNode(WpfObservableRangeCollection<INode> childrenSource, GregoryPatchGenerator dependent, string name) : base(childrenSource, dependent, name)
        {
        }
    }
}
