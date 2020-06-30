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

        public Vector3 GetTangent(float u, float v)
        {
            return new Vector3(
                    (float)(-(MainRadius + TubeRadius * Math.Cos(v)) * Math.Sin(u)),
                    0.0f,
                    (float)((MainRadius + TubeRadius * Math.Cos(v)) * Math.Cos(u))
                );
        }

        public Vector3 GetBitangent(float u, float v)
        {
            return new Vector3(
                    (float)(-TubeRadius * Math.Sin(v) * Math.Cos(u)),
                    (float)(TubeRadius * Math.Cos(v)),
                    (float)(-TubeRadius * Math.Sin(v) * Math.Sin(u))
                );
        }

        public Vector3 GetNormal(float u, float v)
        {
            return Vector3.Cross(GetTangent(u, v), GetBitangent(u, v)).Normalized();
        }
    }
}
