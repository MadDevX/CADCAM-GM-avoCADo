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

        public IList<IntersectionCurve> BoundingCurves { get; } = new List<IntersectionCurve>();

        public TorusSurface(float mainRadius, float tubeRadius)
        {
            MainRadius = mainRadius;
            TubeRadius = tubeRadius;
        }

        public Vector3 GetVertex(float u, float v)
        {
                return new Vector3(
                   (float)((MainRadius + TubeRadius * Math.Cos(v)) * Math.Cos(u)),
                   (float)(TubeRadius * Math.Sin(v)),
                   (float)((MainRadius + TubeRadius * Math.Cos(v)) * Math.Sin(u))
               );
        }

        public Vector3 DerivU(float u, float v)
        {
            return new Vector3(
                    (float)(-(MainRadius + TubeRadius * Math.Cos(v)) * Math.Sin(u)),
                    0.0f,
                    (float)((MainRadius + TubeRadius * Math.Cos(v)) * Math.Cos(u))
                );
        }

        public Vector3 DerivUU(float u, float v)
        {
            return new Vector3(
                    (float)(-(MainRadius + TubeRadius * Math.Cos(v)) * Math.Cos(u)),
                    0.0f,
                    (float)(-(MainRadius + TubeRadius * Math.Cos(v)) * Math.Sin(u))
                );
        }

        public Vector3 DerivV(float u, float v)
        {
            return new Vector3(
                    (float)(-TubeRadius * Math.Sin(v) * Math.Cos(u)),
                    (float)(TubeRadius * Math.Cos(v)),
                    (float)(-TubeRadius * Math.Sin(v) * Math.Sin(u))
                );
        }

        public Vector3 DerivVV(float u, float v)
        {
            return new Vector3(
                    (float)(-TubeRadius * Math.Cos(v) * Math.Cos(u)),
                    (float)(-TubeRadius * Math.Sin(v)),
                    (float)(-TubeRadius * Math.Cos(v) * Math.Sin(u))
                );
        }

        public Vector3 Twist(float u, float v)
        {
            return new Vector3(
                    (float)(TubeRadius * Math.Sin(v) * Math.Sin(u)),
                    0.0f,
                    (float)(-TubeRadius * Math.Sin(v) * Math.Cos(u))
                );
        }

        public Vector3 Normal(float u, float v)
        {
            return Vector3.Cross(DerivU(u, v), DerivV(u, v)).Normalized();
        }
    }
}
