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
        public event Action<INode> ParentChanged;

        public virtual Vector3 Position { get => Vector3.Zero; set { } }
        public Quaternion Rotation { get => Quaternion.Identity; set { } }
        public Vector3 RotationEulerAngles { get => Vector3.Zero; set { } }
        public Vector3 Scale { get => Vector3.One; set { } }

        private INode _parentNode = null;
        public INode ParentNode
        {
            get => _parentNode;
            set
            {
                var prev = _parentNode;
                _parentNode = value;
                ParentChanged?.Invoke(prev);
            }
        }

        private INode _node = null;
        public virtual INode Node
        {
            get => _node;
            set
            {
                if (_node != null)
                {
                    throw new InvalidOperationException("Tried to reattach Transform to a different Node");
                }
                _node = value;
            }
        }

        public Matrix4 LocalModelMatrix => Matrix4.Identity;

        public virtual Vector3 WorldPosition { get => Vector3.Zero; set { } }

        public virtual void RotateAround(Vector3 pivot, Vector3 eulerAngles) { }

        public virtual void ScaleAround(Vector3 pivot, Vector3 scaling) { }

        public virtual Vector2 ScreenCoords(ICamera camera) { return _screenCoords; }

        public virtual void Translate(Vector3 translation) { }

        public virtual Vector3 TranslateSnapped(Vector3 translation, float snapValue) { return Vector3.Zero; }

        private Vector2 _screenCoords = new Vector2(float.MinValue, float.MinValue); // to avoid selecting non-3d object by clicking in the middle of viewport

        protected void RaisePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(sender, e);
        }
    }
}
