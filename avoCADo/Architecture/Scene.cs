using OpenTK;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo
{
    public class Scene : IDisposable, INode
    {
        public string Name { get; set; }
        /// <summary>
        /// Add and remove nodes by dedicated methods (AddNode and DeleteNode)
        /// </summary>
        public ObservableCollection<Node> Children { get; private set; } = new ObservableCollection<Node>();

        public Scene(string name)
        {
            Name = name;
        }

        public void Render(Camera camera)
        {
            for(int i = 0; i < Children.Count; i++)
            {
                Children[i].Render(camera, Matrix4.Identity);
            }
        }

        public void AttachChild(Node child)
        {
            if (child.Parent != null) throw new InvalidOperationException("Tried to attach node that has another parent");

            child.Parent = this;
            Children.Add(child);

        }

        public bool DetachChild(Node child)
        {
            var val = Children.Remove(child);
            if (val) child.Parent = null;
            return val;
        }

        public void Dispose()
        {
            for(int i = Children.Count - 1; i >= 0; i--)
            {
                Children[i].Dispose();
            }
            Children.Clear();
        }
    }
}
