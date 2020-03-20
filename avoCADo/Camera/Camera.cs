using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace avoCADo
{
    public class Camera : IDisposable
    {
        public Vector3 Target => _target;
        public Matrix4 ProjectionMatrix => _projectionMatrix;
        public Matrix4 ViewMatrix => _viewMatrix;


        private ViewportManager _viewportManager;

        private Vector3 _position = Vector3.UnitZ;
        private Vector3 _target = Vector3.Zero;

        private float _yaw;
        private float _pitch;

        private float _nearPlane = 0.01f;
        private float _farPlane = 100.0f;
        private float _fov = MathHelper.DegreesToRadians(90.0f);
        private float _aspectRatio = 1.0f;

        private Matrix4 _projectionMatrix;
        private Matrix4 _viewMatrix;

        public Camera(ViewportManager viewportManager)
        {
            _viewportManager = viewportManager;
            _viewportManager.OnViewportChanged += OnViewportChanged;
            Reset();
        }

        public void Dispose()
        {
            _viewportManager.OnViewportChanged -= OnViewportChanged;
        }

        public void SetCameraMatrices(int shaderHandle)
        {
            SetViewMatrix(shaderHandle);
            SetProjectionMatrix(shaderHandle);
        }

        #region Camera operations

        public void Rotate(float horizontal, float vertical)
        {
            var mag = (_position - _target).Length;
            var xAngleDiff = (float)Math.PI * horizontal;
            var yAngleDiff = -(float)Math.PI * vertical;
            if (Math.Abs(_pitch + yAngleDiff) >= Math.PI * 0.499f) yAngleDiff = 0.0f; //limit camera movement
            _yaw = (_yaw + xAngleDiff) % (2.0f * (float)Math.PI);
            _pitch += yAngleDiff;
            Vector3 direction =
                new Vector3
                (
                    (float)Math.Cos(_yaw) * (float)Math.Cos(_pitch),
                    (float)Math.Sin(_pitch),
                    (float)Math.Sin(_yaw) * (float)Math.Cos(_pitch)
                );
            _position = -direction*mag + _target;
            UpdateViewMatrix();
        }

        public void ChangeFOV(float degrees)
        {
            _fov = MathHelper.Clamp(degrees, MathHelper.DegreesToRadians(1.0f), MathHelper.DegreesToRadians(359.0f));
            UpdateProjectionMatrix();
        }

        public void ChangeOffset(int clicks)
        {
            if(clicks > 0)
            {
                _position += (_target - _position) * 0.1f;
            }
            else if (clicks < 0)
            {
                _position -= (_target - _position) * 0.1f;
            }
            UpdateViewMatrix();
        }

        public void Translate(float horizontal, float vertical)
        {
            var planeTr = new Vector4(-horizontal, vertical, 0.0f, 1.0f);
            var rot = Quaternion.FromMatrix(new Matrix3(Matrix4.LookAt(Vector3.Zero, _target - _position, Vector3.UnitY)));
            var trVec = Vector4.Transform(planeTr, rot);
            var toAdd = new Vector3(trVec.X, trVec.Y, trVec.Z);
            _target += toAdd;
            _position += toAdd;
            UpdateViewMatrix();
        }

        public void Reset()
        {
            _position = Vector3.UnitZ;
            _target = Vector3.Zero;
            _yaw = (float)Math.PI*1.5f;
            _pitch = 0.0f;

            UpdateViewMatrix();
            UpdateProjectionMatrix();
        }

        public void Move(Vector3 target)
        {
            var diff = _position - _target;
            _target = target;
            _position = _target + diff;
            UpdateViewMatrix();
        }

        #endregion

        private void SetViewMatrix(int shaderHandle)
        {
            var location = GL.GetUniformLocation(shaderHandle, "view");
            GL.UniformMatrix4(location, false, ref _viewMatrix);
        }

        private void SetProjectionMatrix(int shaderHandle)
        {
            var location = GL.GetUniformLocation(shaderHandle, "projection");
            GL.UniformMatrix4(location, false, ref _projectionMatrix);
        }

        private void OnViewportChanged(System.Drawing.Size obj)
        {
            _aspectRatio = (float)obj.Width / obj.Height;
            UpdateProjectionMatrix();
        }

        private void UpdateViewMatrix()
        {
            _viewMatrix = Matrix4.LookAt(_position, _target, Vector3.UnitY);
        }

        private void UpdateProjectionMatrix()
        {
            _projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(_fov, _aspectRatio, _nearPlane, _farPlane);
        }
    }
}
