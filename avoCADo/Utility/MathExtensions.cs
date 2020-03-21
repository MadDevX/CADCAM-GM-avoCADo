﻿using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo
{
    public static class MathExtensions
    {
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

    }
}
