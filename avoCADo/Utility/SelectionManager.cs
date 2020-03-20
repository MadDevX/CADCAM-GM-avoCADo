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
        public List<Node> SelectedNodes { get; } = new List<Node>(); //TODO : maybe use ReadOnlyCollection property
        public Node MainSelection { get; private set; } = null;


        public void ResetSelection()
        {
            SelectedNodes.Clear();
            MainSelection = null;
            OnSelectionChanged?.Invoke();
        }

        public void Select(Node node)
        {
            SelectInternal(node);
            OnSelectionChanged?.Invoke();
        }

        public void ToggleSelection(Node node)
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

        private void SelectInternal(Node node)
        {
            SelectedNodes.Clear();
            SelectedNodes.Add(node);
            MainSelection = node;
        }

        private void AddToSelected(Node node)
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

        private void RemoveFromSelected(Node node)
        {
            SelectedNodes.Remove(node);
            if (node == MainSelection && SelectedNodes.Count > 0)
            {
                MainSelection = SelectedNodes[SelectedNodes.Count - 1];
            }
        }
    }
}
