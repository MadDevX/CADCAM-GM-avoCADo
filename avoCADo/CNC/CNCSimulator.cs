using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo.CNC
{
    public class CNCSimulator
    {
        public static void Execute(CNCInstructionSet instructionSet, MaterialBlock block)
        {
            var currentPosition = Vector3.UnitY;
            foreach(var instruction in instructionSet.Instructions)
            {
                block.DrillCircleAtSegment(currentPosition, instruction.Position, instructionSet.Tool);
                currentPosition = instruction.Position;
            }
        }
    }
}
