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
        /// <summary>
        /// Only use for reading data, do not modify this collection. Use dedicated methods instead.
        /// </summary>
        public List<INode> SelectedNodes { get; } = new List<INode>(); //TODO : maybe use ReadOnlyCollection property
        public INode MainSelection { get; private set; } = null;


        public void ResetSelection()
        {
            SelectedNodes.Clear();
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
            if(SelectedNodes.Contains(node))
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
            SelectedNodes.Clear();
            SelectedNodes.Add(node);
            MainSelection = node;
        }

        private void AddToSelected(INode node)
        {
            if (SelectedNodes.Count > 0 && node.Transform.Parent == MainSelection.Transform.Parent)
            {
                SelectedNodes.Add(node);
                MainSelection = node;
            }
            else
            {
                SelectInternal(node);
            }
        }

        private void RemoveFromSelected(INode node)
        {
            SelectedNodes.Remove(node);
            if (node == MainSelection && SelectedNodes.Count > 0)
            {
                MainSelection = SelectedNodes[SelectedNodes.Count - 1];
            }
        }
    }
}
