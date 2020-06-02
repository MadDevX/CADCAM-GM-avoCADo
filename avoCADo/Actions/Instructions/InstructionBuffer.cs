using avoCADo.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo
{
    public class InstructionBuffer
    {
        private List<IInstructionEntry> _issuedInstructions = new List<IInstructionEntry>();

        public void IssueInstruction<TInstruction, TParameters>(TParameters parameters) where TInstruction : Instruction<TParameters>, new()
        {
            var instruction = new TInstruction();
            if (instruction.Execute(parameters))
            {
                _issuedInstructions.Add(instruction);
            }
        }

        public void Undo()
        {
            if (_issuedInstructions.Count == 0) return;
            var lastInstruction = _issuedInstructions[_issuedInstructions.Count - 1];
            if (lastInstruction.Undo())
            {
                _issuedInstructions.RemoveAt(_issuedInstructions.Count - 1);
            }
        }
    }
}
