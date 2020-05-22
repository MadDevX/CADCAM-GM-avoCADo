using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using OpenTK;

namespace avoCADo
{
    public class Node : INode, INotifyPropertyChanged, IDependencyCollector
    {
        public bool IsSelected { get; set; } = false;

        public event PropertyChangedEventHandler PropertyChanged;
        public event Action<INode> OnDisposed;

        public GroupNodeType GroupNodeType => GroupNodeType.None;
        public NodeType NodeType => NodeType.Point;

        public ITransform Transform { get; private set; }
        public IRenderer Renderer { get; private set; }

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

        private static PropertyChangedEventArgs _nameChangedArgs = new PropertyChangedEventArgs(nameof(Name));
        private static PropertyChangedEventArgs _transformChangedArgs = new PropertyChangedEventArgs(nameof(Transform));

        /// <summary>
        /// Do not modify collection through this property - use dedicated methods (AttachChild, DetachChild)
        /// </summary>
        public ObservableCollection<INode> Children { get; private set; } = new ObservableCollection<INode>();

        private Dictionary<DependencyType, List<object>> _dependencies;

        public Matrix4 GlobalModelMatrix
        {
            get
            {
                return Transform.LocalModelMatrix * Transform.Parent.GlobalModelMatrix;
            }
        }

        public Node(Transform transform, IRenderer renderer, string name)
        {
            _dependencies = DictionaryInitializer.InitializeEnumDictionary<DependencyType, List<object>>();
            Transform = transform;
            Renderer = renderer;
            Name = name;
            renderer.SetNode(this);
            Transform.PropertyChanged += TransformModified;
        }

        /// <summary>
        /// Frees resources and detaches itself from parent node.
        /// </summary>
        public virtual void Dispose()
        {
            Transform.PropertyChanged -= TransformModified;
            Renderer.Dispose();
            for (int i = Children.Count - 1; i >= 0; i--)
            {
                Children[i].Dispose();
            }
            Children.Clear();
            OnDisposed?.Invoke(this);
        }

        private void TransformModified(object sender, PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, _transformChangedArgs);
        }

        public void Render(Camera camera, Matrix4 parentMatrix)
        {
            Renderer.Render(camera, Transform.LocalModelMatrix, parentMatrix);
            var modelMat = Transform.LocalModelMatrix * parentMatrix;
            for(int i = 0; i < Children.Count; i++)
            {
                Children[i].Render(camera, modelMat);
            }
        }

        /// <summary>
        /// Attaches child to this node
        /// </summary>
        /// <param name="child"></param>
        public void AttachChild(INode child)
        {
            if (child.Transform.Parent != null) throw new InvalidOperationException("Tried to attach node that has another parent");

            child.Transform.Parent = this;
            Children.Add(child);
            child.OnDisposed += HandleChildDisposed;
        }

        /// <summary>
        /// Detaches child from this node
        /// </summary>
        /// <param name="child"></param>
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

        public void AddDependency(DependencyType type, object dependant)
        {
            if(_dependencies.TryGetValue(type, out var dependencyList))
            {
                dependencyList.Add(dependant);
            }
            else
            {
                throw new InvalidOperationException("Dictionary is not initialized properly - enquired entry was not created");
            }
        }

        public void RemoveDependency(DependencyType type, object dependant)
        {
            if (_dependencies.TryGetValue(type, out var dependencyList))
            {
                dependencyList.Remove(dependant);
            }
            else
            {
                throw new InvalidOperationException("Dictionary is not initialized properly - enquired entry was not created");
            }
        }

        public bool HasDependency(DependencyType type)
        {
            if(_dependencies.TryGetValue(type, out var dependencyList))
            {
                return dependencyList.Count > 0;
            }
            else
            {
                throw new InvalidOperationException("Dictionary is not initialized properly - enquired entry was not created");
            }
        }

        public bool HasDependency()
        {
            return HasDependency(DependencyType.Strong) || HasDependency(DependencyType.Weak);
        }

        protected void InvokeOnDisposed()
        {
            OnDisposed?.Invoke(this);
        }
    }
}
