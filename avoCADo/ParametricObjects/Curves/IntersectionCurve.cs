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

        private List<Vector3> _controlPointsWorld = new List<Vector3>();
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
                _controlPointsWorld.Add(p.GetVertex(pos.X, pos.Y));
                _parameters.Add(pos);
            }
            Refresh();
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
            BernsteinControlPoints.Clear();
            if (_parameters.Count < 2) return;

            for(int i = 0; i < _parameters.Count - 1; i++)
            {
                BernsteinControlPoints.Add(_controlPointsWorld[i]);
                BernsteinControlPoints.Add(Vector3.Lerp(_controlPointsWorld[i], _controlPointsWorld[i + 1], 1.0f / 3.0f));
                BernsteinControlPoints.Add(Vector3.Lerp(_controlPointsWorld[i], _controlPointsWorld[i + 1], 2.0f / 3.0f));
                BernsteinControlPoints.Add(_controlPointsWorld[i+1]);
            }

            for(int i = 0; i < _parameters.Count - 1; i++)
            {
                var p0 = _p.GetVertex(_parameters[i].X, _parameters[i].Y);
                var p3 = _p.GetVertex(_parameters[i + 1].X, _parameters[i + 1].Y);
                BernsteinControlPoints.Add(p0);
                BernsteinControlPoints.Add(Vector3.Lerp(p0, p3, 1.0f / 3.0f));
                BernsteinControlPoints.Add(Vector3.Lerp(p0, p3, 2.0f / 3.0f));
                BernsteinControlPoints.Add(p3);
            }

            for (int i = 0; i < _parameters.Count - 1; i++)
            {
                var q0 = _q.GetVertex(_parameters[i].Z, _parameters[i].W);
                var q3 = _q.GetVertex(_parameters[i + 1].Z, _parameters[i + 1].W);
                BernsteinControlPoints.Add(q0);
                BernsteinControlPoints.Add(Vector3.Lerp(q0, q3, 1.0f / 3.0f));
                BernsteinControlPoints.Add(Vector3.Lerp(q0, q3, 2.0f / 3.0f));
                BernsteinControlPoints.Add(q3);
            }
        }
    }
}
