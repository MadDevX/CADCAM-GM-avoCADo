using avoCADo.HUD;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo.Actions
{
    public class TransformationInstruction : Instruction<TransformationInstruction.Parameters>
    {
        public Vector3 CursorPosition => _parameters.cursorPosition;
        private Parameters _parameters;
        private Vector3 diffVector;

        public override bool Execute(Parameters parameters)
        {
            _parameters = parameters;
            return true;
        }

        public void Append(Vector3 diffVector)
        {
            this.diffVector += diffVector;
        }

        public override bool Undo()
        {
            var _selectionManager = NodeSelection.Manager;
            diffVector = -diffVector; //reverse operation
            switch(_parameters.transformationType)
            {
                case TransformationType.Translation:
                    TransformationsManager.TranslateRaw(_selectionManager.SelectedNodes, diffVector, _parameters.transformationMode, CursorPosition);
                    break;
                case TransformationType.Rotation:
                    TransformationsManager.RotateRaw(_selectionManager.SelectedNodes, diffVector, _parameters.transformationMode, CursorPosition);
                    break;
                case TransformationType.Scale:
                    TransformationsManager.ScaleRaw(_selectionManager.SelectedNodes, diffVector, _parameters.transformationMode, CursorPosition);
                    break;
            }
            return true;
        }

        public struct Parameters
        {
            public TransformationMode transformationMode;
            public TransformationType transformationType;
            public Vector3 cursorPosition;

            public Parameters(TransformationMode transformationMode, TransformationType transformationType, Vector3 cursorPosition)
            {
                this.transformationMode = transformationMode;
                this.transformationType = transformationType;
                this.cursorPosition = cursorPosition;
            }
        }
    }
}
