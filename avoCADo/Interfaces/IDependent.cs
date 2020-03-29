using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo
{
    /// <summary>
    /// Quick hack to resolve circular dependency 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IDependent<T> : IDisposable
    {
        void Initialize(T node);
    }
}
