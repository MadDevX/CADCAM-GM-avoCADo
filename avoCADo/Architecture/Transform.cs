using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using OpenTK;

namespace avoCADo
{
    public class Transform : ITransform
    {
        public Vector3 Position { get; set; } = Vector3.Zero;
        public Vector3 Scale { get; set; } = Vector3.One;
        private Quaternion _rotation = Quaternion.Identity;

        public INode Parent { get; set; }
        public Quaternion Rotation { get => _rotation; set => _rotation = value.Normalized(); }
        public Vector3 RotationEulerAngles
        {
            get
            {
                return Rotation.EulerAngles();
            }
            set
            {
                Rotation = Quaternion.FromEulerAngles(value);
            }
        }

        public Transform(Vector3 position, Vector3 rotation, Vector3 scale)
        {
            Position = position;
            RotationEulerAngles = rotation;
            Scale = scale;
        }

        public Transform(Vector3 position, Quaternion rotation, Vector3 scale)
        {
            Position = position;
            Rotation = rotation;
            Scale = scale;
        }

        public Matrix4 LocalModelMatrix
        {
            get
            {
                var pos = Position;
                Matrix4.CreateTranslation(ref pos, out Matrix4 trans);
                var scl = Scale;
                Matrix4.CreateScale(ref scl, out Matrix4 scale);
                var quat = Rotation;
                Matrix4.CreateFromQuaternion(ref quat, out Matrix4 rotation);
                return scale * rotation * trans;
            }
        }

        public Vector3 WorldPosition
        {
            get
            {
                var vec = new Vector4(Position, 1.0f);
                if(Parent != null) vec = vec * Parent.GlobalModelMatrix;
                return new Vector3(vec.X, vec.Y, vec.Z);
            }
        }

        public Vector2 ScreenCoords(Camera camera)
        {
            Vector4 uni = new Vector4(WorldPosition, 1.0f);
            Vector4 view = uni * camera.ViewMatrix;
            var screenSpace = view * camera.ProjectionMatrix;
            screenSpace /= screenSpace.W;
            return new Vector2(screenSpace.X, screenSpace.Y);
        }

        /// <summary>
        /// Rotates object around a pivot given in local coordinates
        /// </summary>
        /// <param name="pivot">Pivot vector in local coordinates</param>
        /// <param name="eulerAnglesRad">Vector representing rotation around X, Y and Z axes.</param>
        public void RotateAround(Vector3 pivot, Vector3 eulerAnglesRad)
        {
            var diff = Position - pivot;
            var quat = Quaternion.FromEulerAngles(eulerAnglesRad);
            diff = quat * diff;
            Position = pivot + diff;
            Rotation = quat * Rotation;
        }

        public void Translate(Vector3 translation)
        {
            Position += translation;
        }

        public void ScaleAround(Vector3 pivot, Vector3 scaling)
        {
            //TODO : doesn't work yet - diff goes to ridiculously large values and then zeroes itself and stays there (as in MainWindow scenario)
            var diff = Position - pivot;

            diff.X /= this.Scale.X;
            diff.Y /= this.Scale.Y;
            diff.Z /= this.Scale.Z;

            var newScale = this.Scale + scaling;
            diff *= newScale;

            var quat = Rotation;
            quat.Invert();
            var worldScale = quat * this.Scale;
            worldScale.X += scaling.X;
            worldScale.Y += scaling.Y;
            worldScale.Z += scaling.Z;
            this.Scale = Rotation * worldScale;
            Position = pivot + diff;
        }
    }
}
