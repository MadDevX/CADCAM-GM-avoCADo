using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo
{
    public enum DependencyType
    {
        /// <summary>
        /// Object's behaviour should not be affected by any weak dependency by itself
        /// </summary>
        Weak,
        /// <summary>
        /// Object's behaviour can be affected in some way by strong dependency (delete not possible)
        /// </summary>
        Strong
    }

    public interface IDependencyCollector
    {
        int UniqueDependencyCount { get; }
        void AddDependency(DependencyType type, IDependencyAdder dependant);
        void RemoveDependency(DependencyType type, IDependencyAdder dependant);
        bool HasDependency(DependencyType type);
        bool HasDependency();
        bool Contains(IDependencyAdder adder);
        bool HasDependencyOtherThan(IDependencyAdder dependant);

        IList<IDependencyAdder> GetUniqueDependencies(DependencyType type);
        IList<IDependencyAdder> GetNonUniqueDependencies(DependencyType type);
    }
}
