using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace avoCADo
{
    /// <summary>
    /// This node type only groups independent nodes. It does not have scene properties of its own, its only defined by children nodes that are assigned to it.
    /// Moreover, assigning child to GroupNode, does not alter node hierarchy in usual sense - it does not affect Parent node of assigned children.
    /// </summary>
    public abstract class GroupNode<T> : INode, INotifyPropertyChanged where T : IDependent<INode>
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event Action<INode> OnDisposed;

        public bool IsGroupNode => true;

        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Name)));
            }
        }

        public ITransform Transform { get; } = new DummyTransform();

        public IRenderer Renderer { get; }

        public Matrix4 GlobalModelMatrix { get; } = Matrix4.Identity;

        public ObservableCollection<INode> Children { get; } = new ObservableCollection<INode>();


        public GroupNode(IRenderer renderer, T dependent, string name)
        {
            dependent.Initialize(this);
            Renderer = renderer;
            Name = name;
        }

        public void AttachChild(INode node)
        {
            if (Children.Contains(node) == false)
            {
                Children.Add(node);
                node.PropertyChanged += ChildNodeModified;
                node.OnDisposed += HandleChildDisposed;
                if(node.Transform.Parent == null)
                {
                    Transform.Parent.AttachChild(node);
                }
            }
        }

        public bool DetachChild(INode node)
        {
            var res = Children.Remove(node);
            if (res)
            {
                node.Transform.PropertyChanged -= ChildNodeModified;
                node.OnDisposed -= HandleChildDisposed;
            }
            return res;
        }

        private void HandleChildDisposed(INode node)
        {
            DetachChild(node);
        }

        private void ChildNodeModified(object sender, PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Children)));
        }

        public void Dispose()
        {
            for (int i = Children.Count - 1; i >= 0; i--)
            {
                DetachChild(Children[i]);
            }
            Children.Clear();
            Renderer.Dispose();
            OnDisposed?.Invoke(this);
        }

        public void Render(Camera camera, Matrix4 parentMatrix)
        {
            Renderer.Render(camera, Matrix4.Identity, Matrix4.Identity);
        }
    }
}
