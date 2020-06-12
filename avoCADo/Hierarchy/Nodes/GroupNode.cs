using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using OpenTK;

namespace avoCADo
{
    /// <summary>
    /// This node type only groups independent nodes. It does not have scene properties of its own, its only defined by children nodes that are assigned to it.
    /// Moreover, assigning child to GroupNode, does not alter node hierarchy in usual sense - it does not affect Parent node of assigned children.
    /// </summary>
    public abstract class GroupNode<T> : INode, IObject, INotifyPropertyChanged, IDependencyAdder where T : ICircularDependent<INode>
    {
        public bool IsSelectable { get; set; } = true;
        public bool IsSelected { get; set; } = false;
        public event PropertyChangedEventHandler PropertyChanged;
        public event Action<INode> OnDisposed;
        public event Action<IDependencyCollector, IDependencyCollector> DependencyReplaced;

        public ObjectType ObjectType { get; set; }
        public abstract GroupNodeType GroupNodeType { get; }

        /// <summary>
        /// Determines type of dependency added to nodes associated with this GroupNode
        /// </summary>
        public virtual DependencyType ChildrenDependencyType => DependencyType.Weak;

        private static PropertyChangedEventArgs _nameChangedArgs = new PropertyChangedEventArgs(nameof(Name));
        private static PropertyChangedEventArgs _childrenChangedArgs = new PropertyChangedEventArgs(nameof(Children));

        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                PropertyChanged?.Invoke(this, _nameChangedArgs);
            }
        }

        public ITransform Transform { get; } = new GroupTransform();

        public IRenderer Renderer { get; }

        public Matrix4 GlobalModelMatrix { get; } = Matrix4.Identity;

        public ObservableCollection<INode> Children => _children;
        private WpfObservableRangeCollection<INode> _children;

        public GroupNode(WpfObservableRangeCollection<INode> childrenSource, IRenderer renderer, T dependent, string name)
        {
            _children = childrenSource;
            Transform.Node = this;
            dependent.Initialize(this);
            Renderer = renderer;
            Name = name;
            renderer.SetNode(this);
        }

        IList<INode> _nodesToAttachToScene = new List<INode>();
        public void AttachChildRange(IList<INode> nodes)
        {
            _nodesToAttachToScene.Clear();
            foreach (var node in nodes)
            {
                if (Children.Contains(node) == false)
                {
                    node.PropertyChanged += ChildNodeModified;
                    node.OnDisposed += HandleChildDisposed;
                    if (node.Transform.ParentNode == null)
                    {
                        _nodesToAttachToScene.Add(node);
                    }
                    AddDependencyToChild(node);
                }
            }
            _children.AddRange(nodes);
            if (_nodesToAttachToScene.Count > 0)
            {
                Transform.ParentNode.AttachChildRange(_nodesToAttachToScene);
            }
            _nodesToAttachToScene.Clear();
        }

        public void AttachChildAtIndex(INode node, int index)
        {
            if (Children.Contains(node) == false)
            {
                Children.Insert(index, node);
                node.PropertyChanged += ChildNodeModified;
                node.OnDisposed += HandleChildDisposed;
                if (node.Transform.ParentNode == null)
                {
                    Transform.ParentNode.AttachChild(node);
                }
                AddDependencyToChild(node);
            }
        }

        public void AttachChild(INode node)
        {
            if (Children.Contains(node) == false)
            {
                Children.Add(node);
                node.PropertyChanged += ChildNodeModified;
                node.OnDisposed += HandleChildDisposed;
                if(node.Transform.ParentNode == null)
                {
                    Transform.ParentNode.AttachChild(node);
                }
                AddDependencyToChild(node);
            }
        }

        public bool DetachChild(INode node)
        {
            var res = Children.Remove(node);
            if (res)
            {
                node.PropertyChanged -= ChildNodeModified;
                node.OnDisposed -= HandleChildDisposed;
                RemoveDependencyFromChild(node);
            }
            return res;
        }

        public void DetachChildRange(IList<INode> nodes)
        {
            foreach (var child in nodes)
            {
                if (_children.Contains(child))
                {
                    child.PropertyChanged -= ChildNodeModified;
                    child.OnDisposed -= HandleChildDisposed;
                    RemoveDependencyFromChild(child);
                }
            }
            _children.RemoveRange(nodes);
        }

        private void AddDependencyToChild(INode node)
        {
            var depCol = node as IDependencyCollector;
            if (depCol != null)
            {
                depCol.AddDependency(ChildrenDependencyType, this);
            }
        }

        private void RemoveDependencyFromChild(INode node)
        {
            var depCol = node as IDependencyCollector;
            if (depCol != null)
            {
                depCol.RemoveDependency(ChildrenDependencyType, this);
            }
        }

        private void HandleChildDisposed(INode node)
        {
            DetachChild(node);
        }

        private void ChildNodeModified(object sender, PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, _childrenChangedArgs);
        }

        public void Dispose()
        {
            for (int i = Children.Count - 1; i >= 0; i--)
            {
                _children[i].PropertyChanged -= ChildNodeModified;
                _children[i].OnDisposed -= HandleChildDisposed;
                RemoveDependencyFromChild(_children[i]);
            }
            _children.Clear();
            Renderer.Dispose();
            OnDisposed?.Invoke(this);
        }

        public void Render(Camera camera, Matrix4 parentMatrix)
        {
            Renderer.Render(camera, Matrix4.Identity, Matrix4.Identity);
        }

        public void Notify()
        {
            PropertyChanged?.Invoke(this, _childrenChangedArgs);
        }

        public int GetChildIndex(INode node)
        {
            return Children.IndexOf(node);
        }

        public void ReplaceDependency(IDependencyCollector current, IDependencyCollector newDepColl)
        {
            DependencyReplaced?.Invoke(current, newDepColl);
        }
    }
}
