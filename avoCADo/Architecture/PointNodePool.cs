using avoCADo.Constants;
using OpenTK;
using OpenTK.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo.Architecture
{
    public class PointNodePool : IDisposable
    {
        private List<PoolableNode> _pointPool = new List<PoolableNode>(3000);
        private ShaderProvider _shaderProvider;

        public PointNodePool(ShaderProvider shaderProvider)
        {
            _shaderProvider = shaderProvider;
        }

        public INode CreatePointInstance(INode parent, Vector3 position)
        {
            PoolableNode pointNode;

            if (_pointPool.Count == 0)
            {
                pointNode = new PoolableNode(new PointTransform(position, Vector3.Zero, Vector3.One), new PointRenderer(_shaderProvider.DefaultShader, Color4.Orange, Color4.Yellow), NameGenerator.GenerateName(parent, DefaultNodeNames.Point));
                pointNode.ObjectType = ObjectType.Point;
                pointNode.OnReturnToPool += PointNode_OnReturnToPool;
                pointNode.GetFromPool();
            }
            else
            {
                pointNode = _pointPool[_pointPool.Count - 1];
                _pointPool.RemoveAt(_pointPool.Count - 1);
                pointNode.GetFromPool();

                pointNode.Name = NameGenerator.GenerateName(parent, DefaultNodeNames.Point);
                pointNode.Transform.WorldPosition = position;
            }
            return pointNode;
        }

        private void PointNode_OnReturnToPool(PoolableNode node)
        {
            if (node.IsInPool == false)
            {
                _pointPool.Add(node);
                node.ReturnToPool();
            }
        }

        public void Dispose()
        {
            foreach (var node in _pointPool)
            {
                node.OnReturnToPool -= PointNode_OnReturnToPool;
                node.TrueDispose();
            }
            _pointPool.Clear();
        }
    }
}
