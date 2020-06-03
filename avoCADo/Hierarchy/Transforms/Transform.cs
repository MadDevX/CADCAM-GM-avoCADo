using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using OpenTK;

namespace avoCADo
{
    public class Transform : ITransform
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private Vector3 _position = Vector3.Zero;
        private Vector3 _scale = Vector3.One;
        private Quaternion _rotation = Quaternion.Identity;

        private static PropertyChangedEventArgs _positionChangedArgs = new PropertyChangedEventArgs(nameof(Position));
        private static PropertyChangedEventArgs _rotationChangedArgs = new PropertyChangedEventArgs(nameof(Rotation));
        private static PropertyChangedEventArgs _scaleChangedArgs = new PropertyChangedEventArgs(nameof(Scale));

        private ISelectionManager _selectionManager;
        public Vector3 Position
        {
            get => _position;
            set
            {
                _position = value;
                if ((_selectionManager.SelectedNodes.Count < NodeSelection.SensibleSelectionLimit && _selectionManager.SelectedNodes.Count > 0) || 
                    (_selectionManager.MainSelection != null && _selectionManager.MainSelection.Transform == this))
                {
                    PropertyChanged?.Invoke(this, _positionChangedArgs);
                }
            }
        }
        public virtual Vector3 Scale
        {
            get => _scale;
            set
            {
                _scale = value;

                if ((_selectionManager.SelectedNodes.Count < NodeSelection.SensibleSelectionLimit && _selectionManager.SelectedNodes.Count > 0) ||
                    (_selectionManager.MainSelection != null && _selectionManager.MainSelection.Transform == this))
                {
                    PropertyChanged?.Invoke(this, _scaleChangedArgs);
                }
            }
        }
        public virtual Quaternion Rotation
        {
            get => _rotation;
            set
            {
                _rotation = value.Normalized();
                if ((_selectionManager.SelectedNodes.Count < NodeSelection.SensibleSelectionLimit && _selectionManager.SelectedNodes.Count > 0) ||
                    (_selectionManager.MainSelection != null && _selectionManager.MainSelection.Transform == this))
                {
                    PropertyChanged?.Invoke(this, _rotationChangedArgs);
                }
            }
        }
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

        public INode ParentNode { get; set; }

        private INode _node = null;
        public INode Node { 
            get => _node; 
            set
            {
                if(_node != null)
                {
                    throw new InvalidOperationException("Tried to reattach Transform to a different Node");
                }
                _node = value;
            }
        }

        public Transform(Vector3 position, Vector3 rotation, Vector3 scale)
        {
            _selectionManager = NodeSelection.Manager;
            Position = position;
            RotationEulerAngles = rotation;
            Scale = scale;
        }

        public Transform(Vector3 position, Quaternion rotation, Vector3 scale)
        {
            _selectionManager = NodeSelection.Manager;
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
                if(ParentNode != null) vec = vec * ParentNode.GlobalModelMatrix;
                return new Vector3(vec.X, vec.Y, vec.Z);
            }
            set
            {
                var vec = new Vector4(0.0f, 0.0f, 0.0f, 1.0f);
                if (ParentNode != null) vec = vec * ParentNode.GlobalModelMatrix; //TODO: rotate offset
                Position = value - new Vector3(vec.X, vec.Y, vec.Z);
            }
        }

        public Vector2 ScreenCoords(Camera camera)
        {
            return Coordinates.ScreenCoords(camera, WorldPosition);
        }

        /// <summary>
        /// Rotates object around a pivot given in local coordinates
        /// </summary>
        /// <param name="pivot">Pivot vector in local coordinates</param>
        /// <param name="eulerAnglesRad">Vector representing rotation around X, Y and Z axes.</param>
        public void RotateAround(Vector3 pivot, Vector3 eulerAnglesRad)
        {
            //does not work for nested objects
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

        public Vector3 TranslateSnapped(Vector3 translation, float snapValue)
        {
            var newPosition = (Position + translation).RoundToDivisionValue(snapValue);
            var correctedTranslation = newPosition - Position;
            Position = newPosition;
            return correctedTranslation;
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
