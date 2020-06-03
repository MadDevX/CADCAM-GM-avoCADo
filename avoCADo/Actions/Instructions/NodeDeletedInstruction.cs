using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace avoCADo.Actions
{
    public class NodeDeletedInstruction : Instruction<NodeDeletedInstruction.Parameters>
    {
        //TODO: parent should be able to return child's index - that index should be then used for child insertion (i.e. delete middle node from curve, then undo)
        private class DeleteInfo
        {
            public INode _deletedNode = null;
            public INode _deletedNodeParent = null;
            public int _deletedNodeChildIndex;
            public bool _wasSelected = false;
            public List<INode> _depAdders = new List<INode>();
            public int[] _depAddersIndices;
        }

        private DeleteInfo[] _deleteInfos;

        public override bool Execute(Parameters parameters)
        {
            _deleteInfos = new DeleteInfo[parameters.nodes.Count];
            for (int j = 0; j < parameters.nodes.Count; j++)
            {
                var node = parameters.nodes[j];
                _deleteInfos[j] = new DeleteInfo();
                var info = _deleteInfos[j];

                if (node is IDependencyCollector depColl && depColl.HasDependency())
                {
                    DependencyUtility.AddAllDependenciesOfType(info._depAdders, depColl);
                    info._depAddersIndices = new int[info._depAdders.Count];
                }

                for (int i = 0; i < info._depAdders.Count; i++)
                {
                    info._depAddersIndices[i] = info._depAdders[i].GetChildIndex(node);
                    info._depAdders[i].DetachChild(node);
                }

                info._deletedNode = node;
                info._deletedNodeParent = node.Transform.ParentNode;

                if (info._deletedNode.IsSelected)
                {
                    NodeSelection.Manager.ToggleSelection(info._deletedNode);
                    info._wasSelected = true;
                }

                info._deletedNodeChildIndex = info._deletedNodeParent.GetChildIndex(node);
                info._deletedNodeParent.DetachChild(info._deletedNode);
            }
            return true;
        }

        public override bool Undo()
        {
            for(int j = _deleteInfos.Length - 1; j >= 0; j--)
            {
                var info = _deleteInfos[j];
                info._deletedNodeParent.AttachChildAtIndex(info._deletedNode, info._deletedNodeChildIndex);
                for (int i = 0; i < info._depAdders.Count; i++)
                {
                    info._depAdders[i].AttachChildAtIndex(info._deletedNode, info._depAddersIndices[i]);
                }
                if (info._wasSelected) NodeSelection.Manager.ToggleSelection(info._deletedNode);
            }
            return true;
        }

        public override void Dispose()
        {
            foreach (var info in _deleteInfos)
            {
                if (info._deletedNode != null && info._deletedNode.Transform.ParentNode == null)
                {
                    info._deletedNode.Dispose();
                }
            }
        }

        public struct Parameters
        {
            public IList<INode> nodes;

            public Parameters(IList<INode> nodes)
            {
                this.nodes = nodes;
            }
        }
    }
}
