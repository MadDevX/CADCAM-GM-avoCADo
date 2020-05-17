using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo
{
    public class PoolableNode : Node
    {
        public event Action<PoolableNode> OnReturnToPool;

        public PoolableNode(Transform transform, IRenderer renderer, string name) : base(transform, renderer, name)
        {
        }

        public override void Dispose()
        {
            ReturnToPool();
        }

        private void ReturnToPool()
        {
            InvokeOnDisposed();
            OnReturnToPool?.Invoke(this);
        }

        public void TrueDispose()
        {
            base.Dispose();
        }
    }
}
