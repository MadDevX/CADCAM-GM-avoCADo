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
    public class Node : IDisposable, INotifyPropertyChanged, INode
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public Transform Transform { get; private set; }
        public IRenderer Renderer { get; private set; }

        public INode Parent { get; set; }

        private string _name;

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Name)));
            }
        }

        /// <summary>
        /// Do not modify collection through this property - use dedicated methods (AttachChild, DetachChild)
        /// </summary>
        public ObservableCollection<Node> Children { get; private set; } = new ObservableCollection<Node>();


        public Node(Transform transform, IRenderer renderer, string name)
        {
            Transform = transform;
            Renderer = renderer;
            Name = name;
        }

        public void Render(Camera camera, Matrix4 parentMatrix)
        {
            Renderer.Render(Transform, camera, parentMatrix);
            var modelMat =Renderer.GetLocalModelMatrix(Transform) * parentMatrix; //TODO : check matrix multiplication
            for(int i = 0; i < Children.Count; i++)
            {
                Children[i].Render(camera, modelMat);
            }
        }

        /// <summary>
        /// Frees resources and detaches itself from parent node.
        /// </summary>
        public void Dispose()
        {
            if(Parent != null)
            {
                Parent.DetachChild(this);
            }
            Renderer.Dispose();
            for(int i = Children.Count - 1; i >= 0; i--)
            {
                Children[i].Dispose();
            }
            Children.Clear();
        }

        /// <summary>
        /// Attaches child to this node
        /// </summary>
        /// <param name="child"></param>
        public void AttachChild(Node child)
        {
            if (child.Parent != null) throw new InvalidOperationException("Tried to attach node that has another parent");

            child.Parent = this;
            Children.Add(child);
        }

        /// <summary>
        /// Detaches child from this node
        /// </summary>
        /// <param name="child"></param>
        public bool DetachChild(Node child)
        {
            var val = Children.Remove(child);
            if(val) child.Parent = null;
            return val;
        }
    }
}
