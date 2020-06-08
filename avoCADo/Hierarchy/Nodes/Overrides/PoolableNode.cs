using OpenTK;
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
        public bool IsInPool { get; private set; }

        public PoolableNode(Transform transform, IRenderer renderer, string name) : base(transform, renderer, name)
        {
        }

        public override void Dispose()
        {
            NotifyMemoryPool();
        }

        public void GetFromPool()
        {
            IsInPool = false;
        }

        public void ReturnToPool()
        {
            IsInPool = true;
        }

        private void NotifyMemoryPool()
        {
            InvokeOnDisposed();
            OnReturnToPool?.Invoke(this);
            ResetState();
        }

        private void ResetState()
        {
            Transform.Rotation = Quaternion.Identity;
            Transform.Scale = Vector3.One;
            IsSelectable = true;
            if (Transform.ParentNode != null && Transform.ParentNode.DetachChild(this) == false)
            {
                Transform.ParentNode = null;
            }
            foreach (var child in Children) //TODO: if undo for deletion will be available, rework this logic (reset state after creation or remove MemoryPool altogether) (or ignore and disable adding children to point nodes)
            {
                child.Dispose();
            }
            Children.Clear();
        }

        public void TrueDispose()
        {
            base.Dispose();
        }
    }
}
