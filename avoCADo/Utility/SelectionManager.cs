using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo
{
    public class SelectionManager
    {
        public event Action OnSelectionChanged;

        public IReadOnlyCollection<INode> SelectedNodes { get; }
        public INode MainSelection { get; private set; } = null;

        private List<INode> _selectedNodes;

        public SelectionManager()
        {
            _selectedNodes = new List<INode>();
            SelectedNodes = _selectedNodes.AsReadOnly();
        }

        public void ResetSelection()
        {
            _selectedNodes.Clear();
            MainSelection = null;
            OnSelectionChanged?.Invoke();
        }

        public void Select(INode node)
        {
            SelectInternal(node);
            OnSelectionChanged?.Invoke();
        }

        public void ToggleSelection(INode node)
        {
            if(_selectedNodes.Contains(node))
            {
                RemoveFromSelected(node);
            }
            else
            {
                AddToSelected(node);
            }
            OnSelectionChanged?.Invoke();
        }

        private void SelectInternal(INode node)
        {
            _selectedNodes.Clear();
            _selectedNodes.Add(node);
            MainSelection = node;
        }

        private void AddToSelected(INode node)
        {
            if (_selectedNodes.Count > 0 && node.Transform.Parent == MainSelection.Transform.Parent)
            {
                _selectedNodes.Add(node);
                MainSelection = node;
            }
            else
            {
                SelectInternal(node);
            }
        }

        private void RemoveFromSelected(INode node)
        {
            _selectedNodes.Remove(node);
            if (node == MainSelection && _selectedNodes.Count > 0)
            {
                MainSelection = _selectedNodes[_selectedNodes.Count - 1];
            }
        }
    }
}
