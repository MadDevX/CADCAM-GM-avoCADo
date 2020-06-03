using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo
{
    public static class DependencyUtility
    {
        private static Array _dependencyTypes = Enum.GetValues(typeof(DependencyType));
        public static void AddAllDependencies(IList<IDependencyAdder> depAdders, IDependencyCollector depColl)
        {
            depAdders.Clear();
            foreach (DependencyType type in _dependencyTypes)
            {
                foreach (var depAdd in depColl.GetDependencies(type))
                {
                    depAdders.Add(depAdd);
                }
            }
        }

        public static void AddAllDependenciesOfType<T>(IList<T> depAdders, IDependencyCollector depColl)
        {
            depAdders.Clear();
            foreach (DependencyType type in _dependencyTypes)
            {
                foreach (T depAdd in depColl.GetDependencies(type))
                {
                    depAdders.Add(depAdd);
                }
            }
        }
    }
}
