using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo
{
    public static class DictionaryInitializer
    {

        /// <summary>
        /// Initializes dictionary with all entries with keys defined by existing enum type <see cref="TEnumKey"/>. 
        /// For each entry default instance of <see cref="TValue"/> class is created.
        /// </summary>
        /// <typeparam name="TEnumKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        public static Dictionary<TEnumKey, TValue> InitializeEnumDictionary<TEnumKey, TValue>() where TEnumKey : struct, IConvertible
                                                                                 where TValue : new()
        {
            var dict = new Dictionary<TEnumKey, TValue>();
            foreach(TEnumKey key in Enum.GetValues(typeof(TEnumKey)))
            {
                dict.Add(key, new TValue());
            }
            return dict;
        }
    }
}
