using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo
{
    public class Scene : IDisposable
    {
        public string Name { get; set; }
        /// <summary>
        /// Add and remove nodes by dedicated methods (AddNode and DeleteNode)
        /// </summary>
        public ObservableCollection<Node> Nodes { get; private set; } = new ObservableCollection<Node>();

        public Scene(string name)
        {
            Name = name;
        }

        public void Render(Camera camera)
        {
            for(int i = 0; i < Nodes.Count; i++)
            {
                Nodes[i].Render(camera);
            }
        }

        public void AddNode(Node node)
        {
            Nodes.Add(node);
        }

        public void DeleteNode(Node node)
        {
            if(Nodes.Remove(node))
            {
                node.Dispose();
            }
        }

        public void Dispose()
        {
            for(int i = 0; i < Nodes.Count; i++)
            {
                Nodes[i].Dispose();
            }
            Nodes.Clear();
        }
    }
}
