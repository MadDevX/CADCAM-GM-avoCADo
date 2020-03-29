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
    public class Scene : IDisposable, INode
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event Action<INode> OnDisposed;
        public bool IsGroupNode => false;

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
        /// <summary>
        /// Add and remove nodes by dedicated methods (AddNode and DeleteNode)
        /// </summary>
        public ObservableCollection<INode> Children { get; private set; } = new ObservableCollection<INode>();

        public List<INode> VirtualChildren { get; } = new List<INode>();

        public Matrix4 GlobalModelMatrix
        {
            get
            {
                return Matrix4.Identity;
            }
        }

        public ITransform Transform { get; } = new DummyTransform();

        public IRenderer Renderer { get; } = new DummyRenderer();

        public Scene(string name)
        {
            Name = name;
        }

        public void Dispose()
        {
            for (int i = Children.Count - 1; i >= 0; i--)
            {
                Children[i].Dispose();
            }
            Children.Clear();
            for (int i = VirtualChildren.Count - 1; i >= 0; i--)
            {
                VirtualChildren[i].Dispose();
            }
            VirtualChildren.Clear();
            OnDisposed?.Invoke(this);
        }

        public void Render(Camera camera)
        {
            for(int i = 0; i < Children.Count; i++)
            {
                Children[i].Render(camera, Matrix4.Identity);
            }
            for(int i = 0; i < VirtualChildren.Count; i++)
            {
                VirtualChildren[i].Render(camera, Matrix4.Identity);
            }
        }

        public void AttachChild(INode child)
        {
            if (child.Transform.Parent != null) throw new InvalidOperationException("Tried to attach node that has another parent");

            var childList = GetChildList(child);

            child.Transform.Parent = this;
            childList.Add(child);
            child.OnDisposed += HandleChildDisposed;
        }

        public bool DetachChild(INode child)
        {
            var childList = GetChildList(child);

            var val = childList.Remove(child);
            if (val)
            {
                child.Transform.Parent = null;
                child.OnDisposed -= HandleChildDisposed;
            }
            return val;
        }

        private IList<INode> GetChildList(INode child)
        {
            if (child is VirtualNode)
            {
                return VirtualChildren;
            }
            else
            {
                return Children;
            }
        }

        private void HandleChildDisposed(INode node)
        {
            DetachChild(node);
        }

        public void Render(Camera camera, Matrix4 parentMatrix){}
    }
}
