using OpenTK;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace avoCADo
{
    public class Scene : IDisposable, INode
    {
        public bool IsSelected { get; set; } = false;
        public event PropertyChangedEventHandler PropertyChanged;
        public event Action<INode> OnDisposed;
        public GroupNodeType GroupNodeType => GroupNodeType.None;
        public NodeType NodeType => NodeType.Scene;

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
        public ObservableCollection<INode> Children => _children;
        private WpfObservableRangeCollection<INode> _children = new WpfObservableRangeCollection<INode>();

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
            while(Children.Count > 0)
            {
                Children[Children.Count - 1].Dispose();
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

            var childList = GetChildListType(child);

            child.Transform.Parent = this;
            childList.Add(child);
            child.OnDisposed += HandleChildDisposed;
        }

        public bool DetachChild(INode child)
        {
            var childList = GetChildListType(child);

            var val = childList.Remove(child);
            if (val)
            {
                child.Transform.Parent = null;
                child.OnDisposed -= HandleChildDisposed;
            }
            return val;
        }

        /// <summary>
        /// Children must be all Nodes or all VirtualNodes, mixing them together will result in undefined behaviour.
        /// </summary>
        /// <param name="children"></param>
        public void AttachChildRange(IList<INode> children)
        {
            for(int i = 0; i < children.Count; i++)
            {
                if (children[i].Transform.Parent != null) throw new InvalidOperationException("Tried to attach node that has another parent");
                children[i].Transform.Parent = this;
                children[i].OnDisposed += HandleChildDisposed;
            }
            if (children[0] is VirtualNode)
            {
                VirtualChildren.AddRange(children);
            }
            else
            {
                _children.AddRange(children);
            }
        }

        /// <summary>
        /// Determines whether to use Children or VirtualChildren collection based on child's type.
        /// </summary>
        /// <param name="child"></param>
        /// <returns></returns>
        private IList<INode> GetChildListType(INode child)
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
