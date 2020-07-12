using avoCADo.ParametricObjects;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo
{
    public class TorusSurface : ISurface
    {
        public event Action ParametersChanged;
        private ITransform _transform;
        private float _mainR;
        private float _tubeR;

        public float MainRadius
        {
            get => _mainR;
            set
            {
                _mainR = value;
                ParametersChanged?.Invoke();
            }
        }

        public float TubeRadius
        {
            get => _tubeR;
            set
            {
                _tubeR = value;
                ParametersChanged?.Invoke();
            }
        }

        public Vector2 ParameterURange { get; } = new Vector2(0.0f, (float)Math.PI * 2.0f);

        public Vector2 ParameterVRange { get; } = new Vector2(0.0f, (float)Math.PI * 2.0f);

        public bool ULoop => true;

        public bool VLoop => true;

        public IList<IntersectionCurveData> BoundingCurves { get; } = new List<IntersectionCurveData>();

        public TorusSurface(float mainRadius, float tubeRadius)
        {
            MainRadius = mainRadius;
            TubeRadius = tubeRadius;
        }

        public void Initialize(ITransform transform)
        {
            _transform = transform;
        }

        public Vector3 GetVertexLocalSpace(float u, float v)
        {
            var uv = ParameterHelper.CorrectUV(this, new Vector2(u, v));
            u = uv.X;
            v = uv.Y;
            return new Vector3(
               (float)((MainRadius + TubeRadius * Math.Cos(v)) * Math.Cos(u)),
               (float)(TubeRadius * Math.Sin(v)),
               (float)((MainRadius + TubeRadius * Math.Cos(v)) * Math.Sin(u))
           );
        }

        public Vector3 GetVertex(float u, float v)
        {
            var uv = ParameterHelper.CorrectUV(this, new Vector2(u, v));
            u = uv.X;
            v = uv.Y;
            var vect = new Vector3(
                   (float)((MainRadius + TubeRadius * Math.Cos(v)) * Math.Cos(u)),
                   (float)(TubeRadius * Math.Sin(v)),
                   (float)((MainRadius + TubeRadius * Math.Cos(v)) * Math.Sin(u))
               );
            return TransformCoord(vect, true);
        }

        private Vector3 TransformCoord(Vector3 vect, bool translate)
        {
            var uni = translate ? 1.0f : 0.0f;
            var vect4 = new Vector4(vect, uni);
            var mat = _transform.LocalModelMatrix;
            var res = vect4 * mat; //TODO: handle nested objects
            return res.Xyz;
        }

        public Vector3 DerivU(float u, float v)
        {
            var uv = ParameterHelper.CorrectUV(this, new Vector2(u, v));
            u = uv.X;
            v = uv.Y;
            var vect = new Vector3(
                    (float)(-(MainRadius + TubeRadius * Math.Cos(v)) * Math.Sin(u)),
                    0.0f,
                    (float)((MainRadius + TubeRadius * Math.Cos(v)) * Math.Cos(u))
                );
            return TransformCoord(vect, false);
        }

        public Vector3 DerivUU(float u, float v)
        {
            var uv = ParameterHelper.CorrectUV(this, new Vector2(u, v));
            u = uv.X;
            v = uv.Y;
            var vect = new Vector3(
                    (float)(-(MainRadius + TubeRadius * Math.Cos(v)) * Math.Cos(u)),
                    0.0f,
                    (float)(-(MainRadius + TubeRadius * Math.Cos(v)) * Math.Sin(u))
                );
            return TransformCoord(vect, false);
        }

        public Vector3 DerivV(float u, float v)
        {
            var uv = ParameterHelper.CorrectUV(this, new Vector2(u, v));
            u = uv.X;
            v = uv.Y;
            var vect = new Vector3(
                    (float)(-TubeRadius * Math.Sin(v) * Math.Cos(u)),
                    (float)(TubeRadius * Math.Cos(v)),
                    (float)(-TubeRadius * Math.Sin(v) * Math.Sin(u))
                );
            return TransformCoord(vect, false);
        }

        public Vector3 DerivVV(float u, float v)
        {
            var uv = ParameterHelper.CorrectUV(this, new Vector2(u, v));
            u = uv.X;
            v = uv.Y;
            var vect = new Vector3(
                    (float)(-TubeRadius * Math.Cos(v) * Math.Cos(u)),
                    (float)(-TubeRadius * Math.Sin(v)),
                    (float)(-TubeRadius * Math.Cos(v) * Math.Sin(u))
                );
            return TransformCoord(vect, false);
        }

        public Vector3 Twist(float u, float v)
        {
            var uv = ParameterHelper.CorrectUV(this, new Vector2(u, v));
            u = uv.X;
            v = uv.Y;
            var vect = new Vector3(
                    (float)(TubeRadius * Math.Sin(v) * Math.Sin(u)),
                    0.0f,
                    (float)(-TubeRadius * Math.Sin(v) * Math.Cos(u))
                );
            return TransformCoord(vect, false);
        }

        public Vector3 Normal(float u, float v)
        {
            var uv = ParameterHelper.CorrectUV(this, new Vector2(u, v));
            u = uv.X;
            v = uv.Y;
            return Vector3.Cross(DerivU(u, v), DerivV(u, v)).Normalized();
        }

        public bool DifferentPatches(float u1, float v1, float u2, float v2) => false;
    }
}
