using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo.Actions
{
    using Parameters = AttachToCurveInstruction.Parameters;

    public class DetachFromCurveInstruction : Instruction<Parameters>
    {
        private Parameters _parameters;
        private int _indexOfChild;

        public override bool Execute(Parameters parameters)
        {
            _parameters = parameters;
            _indexOfChild = _parameters.curveNode.GetChildIndex(_parameters.childNode);
            return _parameters.curveNode.DetachChild(_parameters.childNode);
        }

        public override bool Undo()
        {
            _parameters.curveNode.AttachChildAtIndex(_parameters.childNode, _indexOfChild);
            return true;
        }
    }
}
