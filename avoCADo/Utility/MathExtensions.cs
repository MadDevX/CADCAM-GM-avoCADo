using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo
{
    public static class MathExtensions
    {
        private static Matrix4 _powerToBernsteinMtx = new Matrix4(1.0f,    0.0f,        0.0f,     0.0f,
                                                                  1.0f, 1.0f / 3.0f,    0.0f,     0.0f,
                                                                  1.0f, 2.0f / 3.0f, 1.0f / 3.0f, 0.0f,
                                                                  1.0f,    1.0f,        1.0f,     1.0f);

        public static Vector3 EulerAngles(this Quaternion q)
        {
            var w2 = q.W * q.W;
            var x2 = q.X * q.X; 
            var y2 = q.Y * q.Y; 
            var z2 = q.Z * q.Z; 
            var eX = (float)Math.Atan2(-2 * (q.Y * q.Z - q.W * q.X), w2 - x2 - y2 + z2);
            var eY = (float)Math.Asin(MathHelper.Clamp(2 * (q.X * q.Z + q.W * q.Y), -1.0, 1.0));
            var eZ = (float)Math.Atan2(-2 * (q.X * q.Y - q.W * q.Z), w2 + x2 - y2 - z2);
            return new Vector3(eX, eY, eZ);
        }

        public static void PowerToBernstein(Vector3 a, Vector3 b, Vector3 c, Vector3 d, ref Vector3[] result)
        {
            var x = new Vector4(a.X, b.X, c.X, d.X);
            var y = new Vector4(a.Y, b.Y, c.Y, d.Y);
            var z = new Vector4(a.Z, b.Z, c.Z, d.Z);

            x = PowerToBernstein(x);
            y = PowerToBernstein(y);
            z = PowerToBernstein(z);

            result[0] = new Vector3(x.X, y.X, z.X);
            result[1] = new Vector3(x.Y, y.Y, z.Y);
            result[2] = new Vector3(x.Z, y.Z, z.Z);
            result[3] = new Vector3(x.W, y.W, z.W);

        }

        public static Vector4 PowerToBernstein(Vector4 powerBasisCoefficients)
        {
            return _powerToBernsteinMtx * powerBasisCoefficients;
        }
    }
}
