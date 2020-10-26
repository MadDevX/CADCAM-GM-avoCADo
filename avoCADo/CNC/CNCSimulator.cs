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
                var newPos = currentPosition;
                newPos.X = float.IsNaN(instruction.X) ? currentPosition.X : instruction.X;
                newPos.Y = float.IsNaN(instruction.Y) ? currentPosition.Y : instruction.Y;
                newPos.Z = float.IsNaN(instruction.Z) ? currentPosition.Z : instruction.Z;

                block.DrillCircleAtSegment(currentPosition, newPos, instructionSet.Tool);
                currentPosition = newPos;
            }
        }
    }
}
