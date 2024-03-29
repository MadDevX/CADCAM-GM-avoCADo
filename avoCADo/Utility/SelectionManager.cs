﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo
{
    public class SelectionManager : ISelectionManager
    {
        public event Action OnSelectionChanged;

        public IReadOnlyCollection<INode> SelectedNodes { get; }

        public INode MainSelection { get; private set; } = null;

        private readonly List<INode> _selectedNodes;

        public SelectionManager()
        {
            _selectedNodes = new List<INode>();
            SelectedNodes = _selectedNodes.AsReadOnly();
        }

        /// <summary>
        /// Deselects all selected nodes. Returns buffer containing all previously selected nodes.
        /// </summary>
        /// <returns></returns>
        public void ResetSelection()
        {
            ClearList();
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
            ToggleSelectionInternal(node);
            OnSelectionChanged?.Invoke();
        }

        public void Select(IList<INode> nodes, bool ignoreGroupNodes = false)
        {
            ResetSelection();
            ToggleSelection(nodes, ignoreGroupNodes);
        }

        public void AddToSelection(IList<INode> nodes, bool ignoreGroupNodes = false)
        {
            foreach (var node in nodes)
            {
                if (node.GroupNodeType == GroupNodeType.None || ignoreGroupNodes == false)
                {
                    AddToSelectionInternal(node);
                }
            }
            OnSelectionChanged?.Invoke();
        }

        public void ToggleSelection(IList<INode> nodes, bool ignoreGroupNodes = false)
        {
            foreach(var node in nodes)
            {
                if(node.GroupNodeType == GroupNodeType.None || ignoreGroupNodes == false)
                { 
                    ToggleSelectionInternal(node);
                }
            }
            OnSelectionChanged?.Invoke();
        }

        private void ToggleSelectionInternal(INode node)
        {
            if (node.IsSelected)
            {
                RemoveFromSelected(node);
            }
            else
            {
                AddToSelected(node);
            }
        }

        private void AddToSelectionInternal(INode node)
        {
            if(node.IsSelected == false)
            {
                AddToSelected(node);
            }
        }

        private void AddToSelected(INode node)
        {
            if (_selectedNodes.Count > 0 && node.Transform.ParentNode == MainSelection.Transform.ParentNode)
            {
                AddToList(node);
                MainSelection = node;
            }
            else
            {
                SelectInternal(node);
            }
        }

        private void SelectInternal(INode node)
        {
            ClearList();
            AddToList(node);
            MainSelection = node;
        }

        private void RemoveFromSelected(INode node)
        {
            RemoveFromList(node);
            if (node == MainSelection)
            {
                if (_selectedNodes.Count > 0)
                {
                    MainSelection = _selectedNodes[_selectedNodes.Count - 1];
                }
                else
                {
                    MainSelection = null;
                }
            }
        }

        private void TrackDispose(INode obj)
        {
            RemoveFromSelected(obj);
            OnSelectionChanged?.Invoke();
        }

        private void AddToList(INode node)
        {
            _selectedNodes.Add(node);
            TrackNode(node);
        }

        private void RemoveFromList(INode node)
        {
            _selectedNodes.Remove(node);
            UntrackNode(node);
        }

        private void TrackNode(INode node)
        {
            node.OnDisposed += TrackDispose;
            node.IsSelected = true;
        }
        private void UntrackNode(INode node)
        {
            node.OnDisposed -= TrackDispose;
            node.IsSelected = false;
        }

        private void ClearList()
        {
            foreach(var node in _selectedNodes)
            {
                UntrackNode(node);
            }
            _selectedNodes.Clear();
        }
    }
}
