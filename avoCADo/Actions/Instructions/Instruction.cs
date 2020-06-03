using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo.Actions
{
    public abstract class Instruction<T> : IInstruction<T>
    {
        protected IInstructionBuffer _instructionBuffer = null;
        public void SetInstructionBuffer(IInstructionBuffer instructionBuffer)
        {
            if (_instructionBuffer != null) throw new InvalidOperationException("InstructionBuffer was already set!");
            _instructionBuffer = instructionBuffer;
        }

        public abstract bool Execute(T parameters);

        public abstract bool Undo();

        public virtual void Dispose()
        {
        }
    }
}
