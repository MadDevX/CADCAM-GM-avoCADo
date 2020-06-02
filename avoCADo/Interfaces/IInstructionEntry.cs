﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo
{
    public interface IInstruction<TExecutionParameters> : IInstructionEntry
    {
        /// <summary>
        /// Executes specified instruction with given parameters. Returns true if instruction was executed successfully.
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns>Value representing whether instruction was executed successfully.</returns>
        bool Execute(TExecutionParameters parameters);
    }

    public interface IInstructionEntry
    {
        /// <summary>
        /// Rollbacks issued instruction. Returns true if rollback is completed successfully.
        /// </summary>
        /// <returns>Value representing whether rollback was completed successfully.</returns>
        bool Undo();
        
        //void Reapply(); //careful now
    }
}
