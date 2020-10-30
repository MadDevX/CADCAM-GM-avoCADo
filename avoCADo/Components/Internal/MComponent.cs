using avoCADo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo.Components
{
    public abstract class MComponent : IMComponent
    {
        public bool Enabled { get; set; } = true;
        public INode OwnerNode { get; private set; } = null;
        public ITransform Transform => OwnerNode.Transform;

        protected bool _initialized { get; private set; } = false;

        public void SetOwnerNode(INode ownerNode)
        {
            if (OwnerNode != null) throw new InvalidOperationException("Tried to reassign Component parent node");
            OwnerNode = ownerNode;
        }

        public virtual void Dispose() { }

        /// <summary>
        /// Put at the end of the overrides
        /// </summary>
        public virtual void Initialize() 
        {
            if (_initialized)
                throw new InvalidOperationException("Component was already initialized!");
            _initialized = true; 
        }
    }
}
