using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo
{
    public class BezierGeomGroupNode : GroupNode<BezierGeneratorGeometry>
    {
        public override NodeType NodeType => NodeType.Curve;
        public BezierGeomGroupNode(ObservableCollection<INode> childrenSource, IRenderer renderer, BezierGeneratorGeometry dependent, string name) : base(childrenSource, renderer, dependent, name)
        {
        }
    }
}
