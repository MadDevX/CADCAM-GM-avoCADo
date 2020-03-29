using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo
{
    public class BezierGroupNode : GroupNode<BezierGeneratorNew>
    {
        public BezierGroupNode(ObservableCollection<INode> childrenSource, IRenderer renderer, BezierGeneratorNew dependent, string name) : base(childrenSource, renderer, dependent, name)
        {
        }
    }

    public class BSplineGroupNode : GroupNode<BSplineGenerator>
    {
        public BSplineGroupNode(ObservableCollection<INode> childrenSource, IRenderer renderer, BSplineGenerator dependent, string name) : base(childrenSource, renderer, dependent, name)
        {
        }
    }
}
