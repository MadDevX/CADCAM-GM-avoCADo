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
        private readonly CNCInstructionSet _instructionSet;
        private readonly MaterialBlock _block;
        private int _curInstruction;
        private float _curDistance;
        public Vector3 CurrentToolPosition { get; private set; }

        public float DistanceToEnd => _instructionSet.PathsLength - _curDistance;

        public CNCSimulator(CNCInstructionSet instructionSet, MaterialBlock block, Vector3 currentToolPosition)
        {
            CurrentToolPosition = currentToolPosition;
            _instructionSet = instructionSet;
            _block = block;
            _curInstruction = 0;
            _block.SetSegmentToDrill(Vector3.UnitY, _instructionSet.Instructions[_curInstruction].Position);
        }

        /// <summary>
        /// Returns true if current simulation finished
        /// </summary>
        /// <param name="distance"></param>
        /// <returns></returns>
        public bool AdvanceSimulation(float distance)
        {
            var remainingDist = distance;
            var insts = _instructionSet.Instructions;

            _curDistance = Math.Min(_instructionSet.PathsLength, _curDistance + remainingDist);
            
            while (remainingDist > 0.0f)
            {
                var ret = _block.AdvanceSegment(remainingDist, _instructionSet.Tool, CurrentToolPosition);
                remainingDist = ret.remainingDist;
                CurrentToolPosition = ret.toolCurrentPosition;

                if (ret.finished)
                {
                    _curInstruction++;
                    if (_curInstruction == insts.Count) return true;
                    _block.SetSegmentToDrill(insts[_curInstruction - 1].Position, insts[_curInstruction].Position);
                }
            }

            return _instructionSet.PathsLength <= _curDistance;
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
