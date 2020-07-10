using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo
{
    public static class Coordinates
    {
        public static Vector2 ScreenCoords(ICamera camera, Vector3 worldPosition)
        {
            Vector4 uni = new Vector4(worldPosition, 1.0f);
            Vector4 view = uni * camera.ViewMatrix;
            var screenSpace = view * camera.ProjectionMatrix;
            screenSpace /= screenSpace.W;
            return new Vector2(screenSpace.X, screenSpace.Y);
        }
    }
}
