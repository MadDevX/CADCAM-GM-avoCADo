using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo
{
    class PointTransform : Transform
    {
        public override Quaternion Rotation { get => Quaternion.Identity; set { } }
        public override Vector3 Scale { get => Vector3.One; set { } }

        public PointTransform(Vector3 position, Vector3 rotation, Vector3 scale) : base(position, rotation, scale)
        {
        }
    }
}
