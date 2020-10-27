using avoCADo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo.Components
{
    public interface IMComponent : IDisposable
    {
        bool Enabled { get; set; }
        INode OwnerNode { get; }
        void SetOwnerNode(INode ownerNode);
        void Initialize();
    }
}
