using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace avoCADo
{
    public class Node : IDisposable
    {
        public Transform Transform { get; private set; }
        public string Name { get; set; }

        /// <summary>
        /// Do not modify collection through this property - use dedicated methods (AttachChild, DetachChild)
        /// </summary>
        public ObservableCollection<Node> Children { get; private set; } = new ObservableCollection<Node>();

        private Renderer _renderer;

        public Node(Transform transform, Renderer renderer, string name)
        {
            Transform = transform;
            _renderer = renderer;
            Name = name;
        }

        public void Render(Camera camera)
        {
            _renderer.Render(Transform, camera);
            var modelMat = _renderer.GetLocalModelMatrix(Transform);
            for (int i = 0; i < Children.Count; i++)
            {
                Children[i].Render(camera, modelMat);
            }
        }

        public void Render(Camera camera, Matrix4 parentMatrix)
        {
            _renderer.Render(Transform, camera, parentMatrix);
            var modelMat = parentMatrix * _renderer.GetLocalModelMatrix(Transform);
            for(int i = 0; i < Children.Count; i++)
            {
                Children[i].Render(camera, modelMat);
            }
        }

        public void Dispose()
        {
            _renderer.Dispose();
        }

        /// <summary>
        /// Attaches child to this node
        /// </summary>
        /// <param name="child"></param>
        public void AttachChild(Node child)
        {
            Children.Add(child);
        }

        /// <summary>
        /// Detaches child from this node
        /// </summary>
        /// <param name="child"></param>
        public void DetachChild(Node child)
        {
            Children.Remove(child);
        }
    }
}
