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
            OnDisposed?.Invoke(this);
        }

        public void Render(Camera camera)
        {
            for(int i = 0; i < Children.Count; i++)
            {
                Children[i].Render(camera, Matrix4.Identity);
            }
        }

        public void AttachChild(INode child)
        {
            if (child.Transform.Parent != null) throw new InvalidOperationException("Tried to attach node that has another parent");

            child.Transform.Parent = this;
            Children.Add(child);
            child.OnDisposed += HandleChildDisposed;
        }

        public bool DetachChild(INode child)
        {
            var val = Children.Remove(child);
            if (val)
            {
                child.Transform.Parent = null;
                child.OnDisposed -= HandleChildDisposed;
            }
            return val;
        }

        private void HandleChildDisposed(INode node)
        {
            DetachChild(node);
        }

        public void Render(Camera camera, Matrix4 parentMatrix){}
    }
}
