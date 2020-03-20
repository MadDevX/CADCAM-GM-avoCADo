using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using OpenTK;

namespace avoCADo
{
    public class Transform
    {
        private Vector3 _rotation = Vector3.Zero;

        public Vector3 position = Vector3.Zero;
        public Vector3 Rotation
        {
            get
            {
                return _rotation;
            }
            set
            {
                _rotation = value;
                CorrectRotation();
            }
        }
        public Vector3 scale = Vector3.One;

        public Transform(){}

        public Transform(Vector3 position, Vector3 rotation, Vector3 scale)
        {
            this.position = position;
            Rotation = rotation;
            this.scale = scale;
        }

        public float CheckDistanceFromScreenCoords(Camera camera, Vector3 mousePosition)
        {
            Vector4 uni = new Vector4(position, 1.0f);
            var screenSpace = camera.ProjectionMatrix * camera.ViewMatrix * uni;

            MessageBox.Show($"Position: {position}\n" +
                            $"Screen: {screenSpace}");

            return 0.0f;
        }


        private void CorrectRotation()
        {
            float pi2 = (float)Math.PI * 2.0f;
            _rotation.X = (_rotation.X + pi2) % pi2;
            _rotation.Y = (_rotation.Y + pi2) % pi2;
            _rotation.Z = (_rotation.Z + pi2) % pi2;
        }
    }
}
