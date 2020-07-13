using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo
{
    public static class VBOUtility
    {
        public static void SetVertex(float[] vertexArray, Vector3 vect, int vertexIndex)
        {
            vertexArray[3 * vertexIndex + 0] = vect.X;
            vertexArray[3 * vertexIndex + 1] = vect.Y;
            vertexArray[3 * vertexIndex + 2] = vect.Z;
        }

        public static void SetVertex(float[] vertexArray, Vector3 vect, Vector2 texCoord, int vertexIndex)
        {
            vertexArray[5 * vertexIndex + 0] = vect.X;
            vertexArray[5 * vertexIndex + 1] = vect.Y;
            vertexArray[5 * vertexIndex + 2] = vect.Z;
            vertexArray[5 * vertexIndex + 3] = texCoord.X;
            vertexArray[5 * vertexIndex + 4] = texCoord.Y;
        }
    }
}
