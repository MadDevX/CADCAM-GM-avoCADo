using avoCADo.HUD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo.Actions
{
    public class TransformationInstructionUtility
    {
        private readonly IInstructionBuffer _instructionBuffer;
        private readonly Cursor3D _cursor;
        private Dictionary<TransformationType, TransformationInstruction> _instructionDict;

        private static Array _types = Enum.GetValues(typeof(TransformationType));

        public TransformationInstructionUtility(IInstructionBuffer instructionBuffer, Cursor3D cursor)
        {
            _instructionDict = DictionaryInitializer.InitializeEnumDictionaryNull<TransformationType, TransformationInstruction>();
            _instructionBuffer = instructionBuffer;
            _cursor = cursor;
        }

        /// <summary>
        /// Creates TransformationInstruction that stores state (checkpoint) - it should be created before any transformation was executed.
        /// </summary>
        /// <param name="currentInstruction"></param>
        public void UpdateInstruction(TransformationMode mode, TransformationType type)
        {
            var currentInstruction = _instructionDict[type];
            if (currentInstruction == null || _instructionBuffer.LastInstruction != currentInstruction || _cursor.Position != currentInstruction.CursorPosition)
            {
                _instructionBuffer.IssueInstruction<TransformationInstruction, TransformationInstruction.Parameters>(
                    new TransformationInstruction.Parameters(mode, type, _cursor.Position));
                _instructionDict[type] = (TransformationInstruction)_instructionBuffer.LastInstruction;
            }
        }

        public void BreakInstructions()
        {
            foreach(TransformationType type in _types)
            {
                _instructionDict[type] = null;
            }
        }
    }
}
