using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo
{
    public interface ISelectionManager
    {
        event Action OnSelectionChanged;
        INode MainSelection { get; }
        IReadOnlyCollection<INode> SelectedNodes { get; }

        void ResetSelection();
        void Select(INode node);
        void Select(IList<INode> nodes, bool ignoreGroupNodes = false);
        void ToggleSelection(INode node);
        void ToggleSelection(IList<INode> nodes, bool ignoreGroupNodes = false);
        void AddToSelection(IList<INode> nodes, bool ignoreGroupNodes = false);
    }
}
