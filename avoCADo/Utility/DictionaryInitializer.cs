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
        /// <br>Initializes dictionary with all entries with keys defined by existing enum type <see cref="TEnumKey"/>.</br>
        /// <br>For each entry default instance of <see cref="TValue"/> class is created.</br>
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

        /// <summary>
        /// <br>Initializes dictionary with all entries with keys defined by existing enum type <see cref="TEnumKey"/>.</br>
        /// <br>For each entry provided instance of <see cref="TValue"/> class is set (in given order!).</br>
        /// </summary>
        /// <typeparam name="TEnumKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="items"></param>
        /// <returns></returns>
        public static Dictionary<TEnumKey, TValue> InitializeEnumDictionary<TEnumKey, TValue>(params TValue[] items) where TEnumKey : struct, IConvertible
        {
            var dict = new Dictionary<TEnumKey, TValue>();
            var keys = Enum.GetValues(typeof(TEnumKey));
            if (keys.Length != items.Length) throw new InvalidOperationException("Provided items list count does not match count of specified enum values");
            for(int i = 0; i < items.Length; i++)
            {
                var key = keys.GetValue(i);
                dict.Add((TEnumKey)key, items[i]);
            }
            return dict;
        }
    }
}
