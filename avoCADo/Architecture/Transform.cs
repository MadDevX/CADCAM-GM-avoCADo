using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace avoCADo
{
    class Transform
    {
        public Vector3 position = Vector3.Zero;
        public Quaternion rotation = Quaternion.Identity;
        public Vector3 scale = Vector3.One;

        public Transform(){}

        public Transform(Vector3 position, Quaternion rotation, Vector3 scale)
        {
            this.position = position;
            this.rotation = rotation;
            this.scale = scale;
        }
    }
}
