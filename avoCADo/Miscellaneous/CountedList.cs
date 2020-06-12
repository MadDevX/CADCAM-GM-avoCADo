using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo.Miscellaneous
{
    /// <summary>
    /// Use only default constructor (do not initialize with existing ienumerable)
    /// Allows adding of non-unique elements, but it is internaly stored as per element count
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CountedList<T> : List<T>
    {
        private List<int> _counts = new List<int>();
        public new void Add(T item)
        {
            var idx = this.IndexOf(item);
            if(idx == -1)
            {
                base.Add(item);
                _counts.Add(1);
            }
            else
            {
                _counts[idx] += 1;
            }
        }

        public int ElementCount(T item)
        {
            var idx = IndexOf(item);
            if (idx == -1) return 0;
            else return ElementCount(idx);
        }

        public int ElementCount(int idx)
        {
            return _counts[idx];
        }

        public new bool Remove(T item)
        {
            var idx = IndexOf(item);
            if (idx != -1)
            {
                RemoveAt(idx);
                return true;
            }
            return false;
        }

        public new void RemoveAt(int idx)
        {
            _counts[idx]--;
            if(_counts[idx] <= 0)
            {
                base.RemoveAt(idx);
                _counts.RemoveAt(idx);
            }
        }
    }
}
