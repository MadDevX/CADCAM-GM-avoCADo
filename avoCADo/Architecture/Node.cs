using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace avoCADo
{
    class Node : IDisposable
    {
        public Transform transform;
        private Renderer _renderer;

        private List<Node> _children = new List<Node>();

        public Node(Transform transform, Renderer renderer)
        {
            this.transform = transform;
            _renderer = renderer;
        }

        public void Render(Camera camera)
        {
            _renderer.Render(transform, camera);
            var modelMat = _renderer.GetLocalModelMatrix(transform);
            for (int i = 0; i < _children.Count; i++)
            {
                _children[i].Render(camera, modelMat);
            }
        }

        public void Render(Camera camera, Matrix4 parentMatrix)
        {
            _renderer.Render(transform, camera, parentMatrix);
            var modelMat = parentMatrix * _renderer.GetLocalModelMatrix(transform);
            for(int i = 0; i < _children.Count; i++)
            {
                _children[i].Render(camera, modelMat);
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
            _children.Add(child);
        }

        /// <summary>
        /// Detaches child from this node
        /// </summary>
        /// <param name="child"></param>
        public void DetachChild(Node child)
        {
            _children.Remove(child);
        }
    }
}
