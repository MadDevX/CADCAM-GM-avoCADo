using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo
{
    public class IntersectionCurve : InterpolatingC2Curve
    {
        private List<Vector3> _controlPoints = new List<Vector3>();
        public override int Segments => ControlPoints.Count - 1;
        public override IList<Vector3> ControlPoints => _controlPoints;

        public IntersectionCurve(IList<INode> knotList) : base(null)
        {
            foreach(var node in knotList)
            {
                ControlPoints.Add(node.Transform.WorldPosition);
            }
        }

        public IntersectionCurve(IList<Vector3> knotList) : base(null)
        {
            foreach(var pos in knotList)
            {
                ControlPoints.Add(pos);
            }
        }
    }
}
