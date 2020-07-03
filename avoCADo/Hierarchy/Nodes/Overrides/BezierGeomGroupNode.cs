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

    public class IntersectionCurveGroupNode : BezierGeomGroupNode
    {
        public override GroupNodeType GroupNodeType => GroupNodeType.Fixed;
        public IntersectionCurveGroupNode(WpfObservableRangeCollection<INode> childrenSource, IRenderer renderer, BezierGeneratorGeometry dependent, string name) : base(childrenSource, renderer, dependent, name)
        {
        }

        public override void Dispose()
        {
            var c = (Renderer.GetGenerator() as BezierGeneratorGeometry).Curve as IntersectionCurve;
            c.Dispose(); //Needs to remove itself from bounding curves of affected surfaces, thus dispose
            base.Dispose(); //Renderer gets disposed here, so before that curve must be disposed
        }
    }
}
