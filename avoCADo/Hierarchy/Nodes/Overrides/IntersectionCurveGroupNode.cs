using avoCADo.Algebra;
using System.Windows.Data;

namespace avoCADo
{
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
