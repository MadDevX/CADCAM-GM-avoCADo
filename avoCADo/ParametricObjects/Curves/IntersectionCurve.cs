using avoCADo.ParametricObjects.Curves;
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
        }

        public IList<Vector2> GetParameterList(ISurface surf, bool selfIntersectionQ = false)
        {
            if (selfIntersectionQ && surf == _p && surf == _q) return _parameters.Select(x => x.Zw).ToList();
            if (surf == _p) return _parameters.Select(x => x.Xy).ToList();
            if (surf == _q) return _parameters.Select(x => x.Zw).ToList();
            throw new InvalidOperationException("Provided surface is not described by this intersection curve");
        }

        public override Vector3 GetVertex(float t)
        {
            int segment = (int)t;
            int startIdx;
            float segmentT;
            if (segment < Segments)
            {
                startIdx = segment * 3;
                segmentT = t % 1.0f;
            }
            else
            {
                startIdx = (segment - 1) * 3;
                segmentT = 1.0f;
            }
            if (BernsteinControlPoints.Count >= startIdx + 4)
            {
                var bStart = BernsteinControlPoints[startIdx];
                var bEnd = BernsteinControlPoints[startIdx + 3];
                return BezierHelper.Bezier(
                               BernsteinControlPoints[startIdx],
                               Vector3.Lerp(bStart, bEnd, 1.0f / 3.0f),
                               Vector3.Lerp(bStart, bEnd, 2.0f / 3.0f),
                               BernsteinControlPoints[startIdx + 3],
                               segmentT
                               );
            }
            throw new Exception("invalid parameter value");
        }

        public override void Refresh()
        {
            base.Refresh();
            for(int i = 0; i + 4 <= BernsteinControlPoints.Count; i += 3)
            {
                BernsteinControlPoints[i + 1] = Vector3.Lerp(BernsteinControlPoints[i], BernsteinControlPoints[i + 3], 1.0f / 3.0f);
                BernsteinControlPoints[i + 2] = Vector3.Lerp(BernsteinControlPoints[i], BernsteinControlPoints[i + 3], 2.0f / 3.0f);
            }
        }
    }
}
