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
        public readonly CNCInstructionSet InstructionSet;

        private readonly MaterialBlock _block;
        private int _curInstruction;
        public Vector3 CurrentToolPosition { get; private set; }

        public int CurrentInstruction => _curInstruction;
        public int InstructionCount => InstructionSet.Instructions.Count;

        public CNCSimulator(CNCInstructionSet instructionSet, MaterialBlock block, Vector3 currentToolPosition)
        {
            CurrentToolPosition = currentToolPosition;
            InstructionSet = instructionSet;
            _block = block;
            _curInstruction = 0;
            _block.SetSegmentToDrill(CurrentToolPosition, InstructionSet.Instructions[_curInstruction].Position);
        }

        /// <summary>
        /// Returns true if current simulation finished
        /// </summary>
        /// <param name="distance"></param>
        /// <returns></returns>
        public bool AdvanceSimulation(float distance)
        {
            var remainingDist = distance;
            var insts = InstructionSet.Instructions;
            
            while (remainingDist > 0.0f)
            {
                var ret = _block.AdvanceSegment(remainingDist, InstructionSet.Tool, CurrentToolPosition);
                remainingDist = ret.remainingDist;
                CurrentToolPosition = ret.toolCurrentPosition;

                if (ret.finished)
                {
                    _curInstruction++;
                    if (_curInstruction == insts.Count) return true;
                    _block.SetSegmentToDrill(insts[_curInstruction - 1].Position, insts[_curInstruction].Position);
                }
            }

            return _curInstruction == insts.Count;
        }

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
