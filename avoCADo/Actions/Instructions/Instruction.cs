using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo.Actions
{
    public abstract class Instruction<T> : IInstruction<T>
    {
        public abstract bool Execute(T parameters);

        public abstract bool Undo();
    }
}
