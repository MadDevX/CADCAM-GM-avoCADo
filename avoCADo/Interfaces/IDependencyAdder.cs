using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo
{
    public interface IDependencyAdder
    {
        /// <summary>
        /// First argument represents DepColl that was replaced, second argument is DepColl that replaces the first one.
        /// </summary>
        event Action<IDependencyCollector, IDependencyCollector> DependencyReplaced;
        DependencyType ChildrenDependencyType { get; }
        void ReplaceDependency(IDependencyCollector current, IDependencyCollector newDepColl);
        void Notify();
    }
}
