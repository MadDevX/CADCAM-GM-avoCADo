using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo.Actions
{
    public class AttachToCurveInstruction : Instruction<AttachToCurveInstruction.Parameters>
    {
        private Parameters _parameters;

        public override bool Execute(Parameters parameters)
        {
            _parameters = parameters;
            _parameters.curveNode.AttachChild(_parameters.childNode);
            return true;
        }

        public override bool Undo()
        {
            _parameters.curveNode.DetachChild(_parameters.childNode);
            return true;
        }

        public struct Parameters
        {
            public INode childNode;
            public INode curveNode;

            public Parameters(INode childNode, INode curveNode)
            {
                this.childNode = childNode;
                this.curveNode = curveNode;
            }
        }
    }
}
