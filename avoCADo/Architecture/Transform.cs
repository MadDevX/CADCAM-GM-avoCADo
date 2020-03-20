﻿using System;
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
        public INode Parent { get; set; }

        private Vector3 _rotation = Vector3.Zero;

        public Vector3 position = Vector3.Zero;

        /// <summary>
        /// In radians
        /// </summary>
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

        public Vector3 WorldPosition
        {
            get
            {
                var vec = new Vector4(position, 1.0f);
                vec = Parent.GlobalModelMatrix * vec;
                return new Vector3(vec.X, vec.Y, vec.Z);
            }
        }

        public float CheckDistanceFromScreenCoords(Camera camera, Vector3 mousePosition)
        {
            Vector4 uni = new Vector4(WorldPosition, 1.0f);
            Vector4 view = uni * camera.ViewMatrix;
            var screenSpace = view * camera.ProjectionMatrix;
            screenSpace /= screenSpace.W;
            Vector2 diff = new Vector2(mousePosition.X - screenSpace.X, mousePosition.Y - screenSpace.Y);
            //MessageBox.Show($"Position: {position}\n" +
            //                $"Screen: {screenSpace}\n" +
            //                $"MousePos: {mousePosition}\n" +
            //                $"Diff: {diff.Length}");
            //MessageBox.Show(camera.ProjectionMatrix.ToString());
            //MessageBox.Show($"{view}");
            return diff.Length;
        }


        private void CorrectRotation()
        {
            float pi2 = (float)Math.PI * 2.0f;
            _rotation.X = (_rotation.X + pi2) % pi2;
            _rotation.Y = (_rotation.Y + pi2) % pi2;
            _rotation.Z = (_rotation.Z + pi2) % pi2;
        }

        /// <summary>
        /// Rotates object around a pivot given in local coordinates
        /// </summary>
        /// <param name="pivot">Pivot vector in local coordinates</param>
        /// <param name="eulerAnglesRad">Vector representing rotation around X, Y and Z axes.</param>
        public void RotateAround(Vector3 pivot, Vector3 eulerAnglesRad)
        {
            var prevRot = Rotation;
            var diff = position - pivot;
            var quat = Quaternion.FromEulerAngles(eulerAnglesRad);
            diff = quat * diff;
            position = pivot + diff;
            Rotation = (quat * Quaternion.FromEulerAngles(Rotation)).EulerAngles();
            if (float.IsNaN(Rotation.X)) Rotation = prevRot;
        }

        public void ScaleAround(Vector3 pivot, Vector3 scale)
        {
            //TODO : doesn't work yet - diff goes to ridiculously large values and then zeroes itself and stays there (as in MainWindow scenario)
            var diff = position - pivot;
            var rotScale = Quaternion.FromEulerAngles(Rotation) * scale;
            diff.X *= scale.X;
            diff.Y *= scale.Y;
            diff.Z *= scale.Z;
            this.scale.X = rotScale.X;
            this.scale.Y = rotScale.Y;
            this.scale.Z = rotScale.Z;
            position = pivot + diff;
        }
    }
}