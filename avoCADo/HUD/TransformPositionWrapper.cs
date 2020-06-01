using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo.HUD
{
    public class TransformPositionWrapper
    {
        public float X { get => _transform.Position.X; set => _transform.Position = new Vector3(value, _transform.Position.Y, _transform.Position.Z); }
        public float Y { get => _transform.Position.Y; set => _transform.Position = new Vector3(_transform.Position.X, value, _transform.Position.Z); }
        public float Z { get => _transform.Position.Z; set => _transform.Position = new Vector3(_transform.Position.X, _transform.Position.Y, value); }


        private readonly Transform _transform;
        public TransformPositionWrapper(Transform transform)
        {
            _transform = transform;
        }
    }
}
