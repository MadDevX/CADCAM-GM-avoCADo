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
        //Overrides seem like a good idea for points, but on the other hand, it takes away some of the flexibility (snapped scaling/rotation, proper nesting of nodes (in the future))

        //public override Quaternion Rotation { get => Quaternion.Identity; set { } }
        //public override Vector3 Scale { get => Vector3.One; set { } }

        public PointTransform(Vector3 position, Vector3 rotation, Vector3 scale) : base(position, rotation, scale)
        {
        }
    }
}
