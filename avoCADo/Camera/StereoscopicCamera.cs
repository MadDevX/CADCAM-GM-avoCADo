using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo
{

    public class StereoscopicCamera : Camera
    {
        public enum RenderMode
        {
            Standard = 0,
            Stereoscopic = 1
        }

        public enum RenderCycle
        {
            LeftEye = 0,
            RightEye = 1
        }

        public float FocusPlaneDistance { get; set; } = 2.0f;
        public float EyeDistance { get; set; } = 0.05f;

        public RenderMode Mode { get; set; } = RenderMode.Standard;

        public override int Cycles => Mode == RenderMode.Stereoscopic ? 2 : 1;

        public override Color4 FilterColor
        {
            get
            {
                if (Mode == RenderMode.Standard) return Color4.White;
                else//if (Mode == RenderMode.Stereoscopic)
                {
                    if (GetCycle() == RenderCycle.RightEye) return Color4.Cyan;
                    else /*if (GetCycle() == RenderCycle.LeftEye)*/ return Color4.Red;
                }
            }
        }

        public StereoscopicCamera(ViewportManager viewportManager) : base(viewportManager)
        {
        }

        public override void SetCameraMatrices(ShaderWrapper shaderWrapper)
        {
            UpdateViewMatrix();
            UpdateProjectionMatrix();
            base.SetCameraMatrices(shaderWrapper);
        }

        protected override void UpdateViewMatrix()
        {
            switch(Mode)
            {
                case RenderMode.Standard:
                    {
                        base.UpdateViewMatrix();
                        break;
                    }
                case RenderMode.Stereoscopic:
                    {
                        UpdateStereoscopicViewMatrix();
                        break;
                    }
            }
        }

        protected override void UpdateProjectionMatrix()
        {
            switch (Mode)
            {
                case RenderMode.Standard:
                    {
                        base.UpdateProjectionMatrix();
                        break;
                    }
                case RenderMode.Stereoscopic:
                    {
                        UpdateStereoscopicProjectionMatrix();
                        break;
                    }
            }
        }

        private void UpdateStereoscopicViewMatrix()
        {
            var offset = new Vector4(CalculateEyeOffset(GetCycle()), 0.0f, 0.0f, 1.0f);
            var rot = CalculateCameraRotation();
            offset = Vector4.Transform(offset, rot);
            var offset3 = offset.Xyz;
            _viewMatrix = Matrix4.LookAt(_position + offset3, _target + offset3, Vector3.UnitY);
        }

        private void UpdateStereoscopicProjectionMatrix()
        {
            var top = CalculateViewFrustrumTop(_fov, _nearPlane);

            var offset = -CalculateEyeOffset(GetCycle());

            offset *= _nearPlane / FocusPlaneDistance;

            _projectionMatrix = Matrix4.CreatePerspectiveOffCenter(-_aspectRatio * top + offset, _aspectRatio * top + offset, -top, top, _nearPlane, _farPlane);
        }

        /// <summary>
        /// Calculates value representing top of the camera view frustrum 
        /// [(-top) == bottom ||
        /// _aspectRatio * top ==  right || 
        /// (-top) * _aspectRatio == left]
        /// </summary>
        /// <returns></returns>
        private float CalculateViewFrustrumTop(float fieldOfView, float nearPlane)
        {
            return (float)(1.0 / Math.Tan(fieldOfView / 2.0f)) * nearPlane;
        }

        /// <summary>
        /// Returns currently set camera eye offset (half of eye distance with proper direction - negative for left eye, positive for right eye).
        /// </summary>
        /// <returns></returns>
        private float CalculateEyeOffset(RenderCycle cycle)
        {
            var offset = EyeDistance * 0.5f;

            if (cycle == RenderCycle.LeftEye) offset *= -1.0f;
            else if (cycle == RenderCycle.RightEye) offset *= 1.0f;
            return offset;
        }

        private RenderCycle GetCycle()
        {
            return (RenderCycle)_currentCycle;
        }

        //private Matrix4 GetStereoscopicProjectionMatrix(float n, float f, float l, float r, float t, float b)
        //{
        //    return new Matrix4
        //        (
        //            2.0f * n / (r - l), 0.0f, (r + l) / (r - l), 0.0f,
        //            0.0f, 2.0f * n / (t - b), (t + b) / (t - b), 0.0f,
        //            0.0f, 0.0f, (f + n) / (f - n), (-2.0f * f * n) / (f - n),
        //            0.0f, 0.0f, 1.0f, 0.0f
        //        );
        //}
    }
}
