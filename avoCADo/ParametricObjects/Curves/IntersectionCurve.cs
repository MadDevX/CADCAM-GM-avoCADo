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
        private ISurface _p;
        private ISurface _q;
        private List<Vector4> _parameters = new List<Vector4>();
        public IReadOnlyCollection<Vector4> Parameters => _parameters.AsReadOnly();

        public IntersectionCurve(IList<INode> knotList) : base(null)
        {
            foreach(var node in knotList)
            {
                ControlPoints.Add(node.Transform.WorldPosition);
            }
        }

        public IntersectionCurve(ISurface p, ISurface q, IList<Vector4> uvstParameters) : base(null)
        {
            _p = p;
            _q = q;
            foreach(var pos in uvstParameters)
            {
                ControlPoints.Add(p.GetVertex(pos.X, pos.Y));
                _parameters.Add(pos);
            }

            _p.BoundingCurves.Add(this);
            _q.BoundingCurves.Add(this);
        }

        public IList<Vector2> GetParameterList(ISurface surf)
        {
            if (surf == _p) return _parameters.Select(x => x.Xy).ToList();
            if (surf == _q) return _parameters.Select(x => x.Zw).ToList();
            throw new InvalidOperationException("Provided surface is not described by this intersection curve");
        }

        public void Dispose()
        {
            _p.BoundingCurves.Remove(this);
            _q.BoundingCurves.Remove(this);
        }
    }
}
