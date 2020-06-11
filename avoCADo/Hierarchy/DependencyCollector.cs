using avoCADo.Miscellaneous;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo
{
    public class DependencyCollector : IDependencyCollector
    {

        private Dictionary<DependencyType, CountedList<IDependencyAdder>> _dependencies;

        public DependencyCollector()
        {
            _dependencies = DictionaryInitializer.InitializeEnumDictionary<DependencyType, CountedList<IDependencyAdder>>();
        }

        public void AddDependency(DependencyType type, IDependencyAdder dependant)
        {
            _dependencies[type].Add(dependant);
        }

        public void RemoveDependency(DependencyType type, IDependencyAdder dependant)
        {
            _dependencies[type].Remove(dependant);
        }

        public bool HasDependency(DependencyType type)
        {
            return _dependencies[type].Count > 0;
        }

        public bool HasDependency()
        {
            return HasDependency(DependencyType.Strong) || HasDependency(DependencyType.Weak);
        }

        public bool HasDependencyOtherThan(IDependencyAdder depAdd)
        {
            var strongList = _dependencies[DependencyType.Strong];
            var weakList = _dependencies[DependencyType.Weak];

            var strongCount = strongList.Contains(depAdd) ? strongList.Count - 1 : strongList.Count;
            var weakCount = weakList.Contains(depAdd) ? weakList.Count - 1 : weakList.Count;

            return strongCount + weakCount > 0;
        }

        public IList<IDependencyAdder> GetDependencies(DependencyType type)
        {
            return _dependencies[type];
        }
    }
}
