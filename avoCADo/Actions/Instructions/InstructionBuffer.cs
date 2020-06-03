using avoCADo.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace avoCADo
{
    public class InstructionBuffer : IInstructionBuffer
    {
        public IInstructionEntry LastInstruction => _issuedInstructions.Count > 0 ? _issuedInstructions[_issuedInstructions.Count - 1] : null;

        private List<IInstructionEntry> _issuedInstructions = new List<IInstructionEntry>();

        public void IssueInstruction<TInstruction, TParameters>(TParameters parameters) where TInstruction : Instruction<TParameters>, new()
        {
            var instruction = new TInstruction();
            instruction.SetInstructionBuffer(this);
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
                lastInstruction.Dispose();
            }
            else
            {
                MessageBox.Show($"Undo of {lastInstruction.GetType()} failed!");
            }
        }

        public void Clear()
        {
            foreach(var instruction in _issuedInstructions)
            {
                instruction.Dispose();
            }
            _issuedInstructions.Clear();
        }
    }
}
