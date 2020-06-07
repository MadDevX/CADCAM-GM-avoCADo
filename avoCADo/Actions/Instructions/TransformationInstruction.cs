using avoCADo.Architecture;
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
        private ISelectionManager _selectionManager;
        private DependencyAddersManager _dependencyAddersManager;
        private List<(Vector3 pos, Quaternion rot, Vector3 scl)> _startingStates;
        public override bool Execute(Parameters parameters)
        {
            _selectionManager = NodeSelection.Manager;
            _dependencyAddersManager = NodeSelection.DependencyAddersManager;
            _parameters = parameters;
            var selected = _selectionManager.SelectedNodes;
            _startingStates = new List<(Vector3 pos, Quaternion rot, Vector3 scl)>(selected.Count);
            foreach(var node in selected)
            {
                var tr = node.Transform;
                _startingStates.Add((tr.Position, tr.Rotation, tr.Scale));
            }

            return true;
        }

        public override bool Undo()
        {
            int i = 0;
            foreach (var node in _selectionManager.SelectedNodes)
            {
                var state = _startingStates[i];
                var tr = node.Transform;
                tr.Position = state.pos;
                tr.Rotation = state.rot;
                tr.Scale = state.scl;
                i++;
            }
            _dependencyAddersManager.NotifyDependencyAdders();
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
