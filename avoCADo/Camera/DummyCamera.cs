using OpenTK;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo
{
    public class DummyCamera : ICamera
    {
        public Vector3 Target => Vector3.Zero;

        public Vector3 Position => Vector3.Zero;

        public float DistanceToTarget => 0.0f;

        public float Pitch => 0.0f;

        public float NearPlane => 0.0f;

        public Matrix4 ProjectionMatrix => Matrix4.Identity;

        public Matrix4 ViewMatrix => Matrix4.Identity;

        public void Move(Vector3 target)
        {
        }

        public void SetCameraMatrices(ShaderWrapper shaderWrapper)
        {
            shaderWrapper.SetViewMatrix(ViewMatrix);
            shaderWrapper.SetProjectionMatrix(ProjectionMatrix);
        }

        public Vector3 ViewPlaneVectorToWorldSpace(Vector2 vect)
        {
            return new Vector3(vect.X, vect.Y, 0.0f);
        }

        public Vector3 ViewPlaneVectorToWorldSpace(float x, float y)
        {
            return new Vector3(x, y, 0.0f);
        }
    }
}
