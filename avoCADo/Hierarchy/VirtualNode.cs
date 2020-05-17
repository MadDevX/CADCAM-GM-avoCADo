using OpenTK;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo
{
    /// <summary>
    /// Does not show up in hierarchy, cannot have child nodes
    /// </summary>
    public class VirtualNode : INode, INotifyPropertyChanged
    {
        public GroupNodeType GroupNodeType => GroupNodeType.None;
        public NodeType NodeType => NodeType.Point;

        public string Name { get; set; }

        public ITransform Transform { get; }

        public IRenderer Renderer { get; }

        public Matrix4 GlobalModelMatrix
        {
            get
            {
                return Transform.LocalModelMatrix * Transform.Parent.GlobalModelMatrix;
            }
        }

        public ObservableCollection<INode> Children { get; } = new ObservableCollection<INode>();

        public event Action<INode> OnDisposed;
        public event PropertyChangedEventHandler PropertyChanged;

        public VirtualNode(Transform transform, IRenderer renderer)
        {
            Transform = transform;
            Renderer = renderer;
            Name = "";
            Transform.PropertyChanged += TransformModified;
        }

        public void AttachChild(INode node)
        {
        }

        public bool DetachChild(INode node)
        {
            return false;
        }

        public void Dispose()
        {
            Transform.PropertyChanged -= TransformModified;
            Renderer.Dispose();
            OnDisposed?.Invoke(this);
        }

        private void TransformModified(object sender, PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Transform)));
        }

        public void Render(Camera camera, Matrix4 parentMatrix)
        {
            Renderer.Render(camera, Transform.LocalModelMatrix, parentMatrix);
        }
    }
}
