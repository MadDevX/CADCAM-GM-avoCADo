using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo
{
    class Scene : IDisposable
    {
        private List<Node> _nodes = new List<Node>();

        public void Render(Camera camera)
        {
            for(int i = 0; i < _nodes.Count; i++)
            {
                _nodes[i].Render(camera);
            }
        }

        public void AddNode(Node node)
        {
            _nodes.Add(node);
        }

        public void DeleteNode(Node node)
        {
            if(_nodes.Remove(node))
            {
                node.Dispose();
            }
        }

        public void Dispose()
        {
            for(int i = 0; i < _nodes.Count; i++)
            {
                _nodes[i].Dispose();
            }
            _nodes.Clear();
        }
    }
}
