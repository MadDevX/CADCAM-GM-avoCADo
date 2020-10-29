using avoCADo.Algebra;
using System.Windows.Data;

namespace avoCADo
{
    public class IntersectionCurveGroupNode : BezierGeomGroupNode
    {
        public override GroupNodeType GroupNodeType => GroupNodeType.Fixed;

        private IntersectionCurveData _curveData;
        private INode _pNode;
        private INode _qNode;
        private IntersectionData _intersectionData;

        public IntersectionCurveGroupNode(WpfObservableRangeCollection<INode> childrenSource, BezierGeneratorGeometry dependent, IntersectionCurve curve, INode pNode, INode qNode, ISurface p, ISurface q, string name) : base(new DummyTransform(), childrenSource, dependent, name)
        {
            _pNode = pNode;
            _qNode = qNode;
            _intersectionData = new IntersectionData(p, q);
            _curveData = new IntersectionCurveData(this, curve);

            _intersectionData.p.BoundingCurves.Add(_curveData);
            _intersectionData.q.BoundingCurves.Add(_curveData);

            //_pNode.PropertyChanged += HandleChange;
            //_qNode.PropertyChanged += HandleChange;
        }

        public void ForceUpdate()
        {
            RaisePropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(nameof(Name)));
        }

        private void HandleChange(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(this, e);
        }

        public override void Dispose()
        {
            //_pNode.PropertyChanged -= HandleChange;
            //_qNode.PropertyChanged -= HandleChange;
            _intersectionData.p.BoundingCurves.Remove(_curveData);
            _intersectionData.q.BoundingCurves.Remove(_curveData);
            _intersectionData.p.TrimTexture.UpdateTrimTexture(_intersectionData.q, true);
            _intersectionData.q.TrimTexture.UpdateTrimTexture(_intersectionData.p, false);
            base.Dispose(); //Renderer gets disposed here, so before that curve must be disposed
        }
    }
}
