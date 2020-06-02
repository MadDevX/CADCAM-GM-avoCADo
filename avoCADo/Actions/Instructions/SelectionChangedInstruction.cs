using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo.Actions
{
    public class SelectionChangedInstruction : Instruction<SelectionChangedInstruction.Parameters>
    {
        private List<INode> _previousSelection = new List<INode>();
        private Parameters _instructionParameters;
        private ISelectionManager _manager;

        public override bool Execute(Parameters parameters)
        {
            SaveData(parameters);

            switch (parameters.operationType)
            {
                case OperationType.Select:
                    _manager.Select(parameters.nodes, parameters.ignoreGroupNodes);
                    break;
                case OperationType.Reset:
                    _manager.ResetSelection();
                    break;
                case OperationType.ToggleSelect:
                    _manager.ToggleSelection(parameters.nodes, parameters.ignoreGroupNodes);
                    break;
            }

            return true;
        }

        public override bool Undo()
        {
            _manager.Select(_previousSelection);
            return true;
        }

        private void SaveData(Parameters parameters)
        {
            _manager = NodeSelection.Manager;
            _instructionParameters = parameters;
            _previousSelection.AddRange(_manager.SelectedNodes);
        }


        public struct Parameters
        {
            public IList<INode> nodes;
            public OperationType operationType;
            public bool ignoreGroupNodes;

            public Parameters(IList<INode> nodes, OperationType type, bool ignoreGroupNodes = false)
            {
                this.nodes = nodes;
                this.operationType = type;
                this.ignoreGroupNodes = ignoreGroupNodes;
            }
        }

        public enum OperationType
        {
            Select,
            ToggleSelect,
            Reset
        }
    }
}
