using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace avoCADo
{
    public class DummyTransform : ITransform
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public Vector3 Position { get => Vector3.Zero; set { } }
        public Quaternion Rotation { get => Quaternion.Identity; set { } }
        public Vector3 RotationEulerAngles { get => Vector3.Zero; set { } }
        public Vector3 Scale { get => Vector3.One; set { } }
        public INode Parent { get; set; }

        public Matrix4 LocalModelMatrix => Matrix4.Identity;

        public Vector3 WorldPosition => Vector3.Zero;

        public void RotateAround(Vector3 pivot, Vector3 eulerAngles) { }

        public void ScaleAround(Vector3 pivot, Vector3 scaling) { }

        public Vector2 ScreenCoords(Camera camera) { return _screenCoords; }

        public void Translate(Vector3 translation) { }

        private Vector2 _screenCoords = new Vector2(float.MinValue, float.MinValue); // to avoid selecting non-3d object by clicking in the middle of viewport
    }
}
