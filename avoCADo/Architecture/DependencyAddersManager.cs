using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo.Architecture
{
    public class DependencyAddersManager : IDisposable
    {
        private List<IDependencyAdder> _dependencyAddersBuffer = new List<IDependencyAdder>();
        private static Array _dependencyTypes = Enum.GetValues(typeof(DependencyType));
        private readonly SelectionManager _selectionManager;
        public IReadOnlyCollection<IDependencyAdder> DependencyAddersFromCurrentSelection { get; }
        public DependencyAddersManager()
        {
            _selectionManager = NodeSelection.Manager;
            DependencyAddersFromCurrentSelection = _dependencyAddersBuffer.AsReadOnly();
            Initialize();
        }

        private void Initialize()
        {
            _selectionManager.OnSelectionChanged += UpdateDependencyAddersBuffer;
        }

        public void Dispose()
        {
            _selectionManager.OnSelectionChanged -= UpdateDependencyAddersBuffer;
        }


        public void NotifyDependencyAdders()
        {
            foreach (var depAdd in DependencyAddersFromCurrentSelection)
            {
                depAdd.Notify();
            }
        }

        private void UpdateDependencyAddersBuffer()
        {
            _dependencyAddersBuffer.Clear();
            foreach (var node in _selectionManager.SelectedNodes)
            {
                if (node is IDependencyCollector depColl)
                {
                    foreach (DependencyType type in _dependencyTypes)
                    {
                        foreach (var depAdd in depColl.GetDependencies(type))
                        {
                            if (_dependencyAddersBuffer.Contains(depAdd) == false) _dependencyAddersBuffer.Add(depAdd);
                        }
                    }
                }
            }
        }

    }
}
