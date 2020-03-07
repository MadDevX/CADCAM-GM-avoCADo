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
        private Transform _transform;
        private Renderer _renderer;

        //private List<Node> _children = new List<Node>();

        public Node(Transform transform, Renderer renderer)
        {
            _transform = transform;
            _renderer = renderer;
        }

        public void Render()
        {
            _renderer.Render(_transform);
            //for(int i = 0; i < _children.Count; i++)
            //{
            //    _children[i].Render();
            //}
        }

        //public void Render(Matrix4 matrix)
        //{
        //    _
        //}

        public void Dispose()
        {
            _renderer.Dispose();
        }

        ///// <summary>
        ///// Attaches child to this node
        ///// </summary>
        ///// <param name="child"></param>
        //public void AttachChild(Node child)
        //{
        //    _children.Add(child);
        //}

        ///// <summary>
        ///// Detaches child from this node
        ///// </summary>
        ///// <param name="child"></param>
        //public void DetachChild(Node child)
        //{
        //    _children.Remove(child);
        //}
    }
}
