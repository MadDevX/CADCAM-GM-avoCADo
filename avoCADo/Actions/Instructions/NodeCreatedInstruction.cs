using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo.Actions
{
    public class NodeCreatedInstruction : Instruction<NodeCreatedInstruction.Parameters>
    {
        private INode _createdNode;
        private Parameters _parameters;

        public override bool Execute(Parameters parameters)
        {
            var node = parameters.nodeFactory.CreateObject(parameters.objectType, parameters.createParameters);
            SaveData(parameters, node);
            return node != null;
        }

        private void SaveData(Parameters parameters, INode node)
        {
            _parameters = parameters;
            _createdNode = node;
        }

        public override bool Undo()
        {
            if (_createdNode != null)
            {
                _createdNode.Dispose();
                return true;
            }
            else return false;
        }

        public struct Parameters
        {
            public NodeFactory nodeFactory;
            public ObjectType objectType;
            public object createParameters;

            public Parameters(NodeFactory nodeFactory, ObjectType objectType, object createParameters)
            {
                this.nodeFactory = nodeFactory;
                this.objectType = objectType;
                this.createParameters = createParameters;
            }
        }
    }
}
