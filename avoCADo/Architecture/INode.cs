using OpenTK;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo
{
    public interface INode
    {
        void AttachChild(Node node);
        bool DetachChild(Node node);
        
        ObservableCollection<Node> Children { get; }

        Matrix4 GlobalModelMatrix { get; }
    }
}
