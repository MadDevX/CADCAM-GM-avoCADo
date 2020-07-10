using avoCADo.Algebra;
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

    public class IntersectionCurveData
    {
        public INode Node { get; }
        public IntersectionCurve Curve { get; }

        public IntersectionCurveData(INode node, IntersectionCurve curve)
        {
            Node = node;
            Curve = curve;
        }
    }

    public class IntersectionCurveGroupNode : BezierGeomGroupNode
    {
        public override GroupNodeType GroupNodeType => GroupNodeType.Fixed;

        private IntersectionCurveData _curveData;
        private IntersectionData _intersectionData;

        public IntersectionCurveGroupNode(WpfObservableRangeCollection<INode> childrenSource, IRenderer renderer, BezierGeneratorGeometry dependent, IntersectionCurve curve, IntersectionData intersectionData, string name) : base(childrenSource, renderer, dependent, name)
        {
            _intersectionData = intersectionData;
            _curveData = new IntersectionCurveData(this, curve);

            _intersectionData.p.BoundingCurves.Add(_curveData);
            _intersectionData.q.BoundingCurves.Add(_curveData);
        }

        public override void Dispose()
        {
            _intersectionData.p.BoundingCurves.Remove(_curveData);
            _intersectionData.q.BoundingCurves.Remove(_curveData);

            base.Dispose(); //Renderer gets disposed here, so before that curve must be disposed
        }
    }
}
