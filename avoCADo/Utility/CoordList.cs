using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo
{
    public class CoordList<T>
    {
        public int Width { get; private set; }
        public int Height { get; private set; }
        public int Count => Width * Height;
        private List<T> _list;

        public CoordList()
        {
            _list = new List<T>();
            Width = Height = 0;
        }

        public CoordList(IList<T> list, int width, int height)
        {
            _list = new List<T>(width * height);
            SetData(list, width, height);
        }

        public void SetData(IList<T> list, int width, int height)
        {
            if (list.Count != width * height)
            {
                throw new InvalidOperationException("Provided list does not match provided dimensions");
            }
            _list.Clear();
            _list.AddRange(list);
            Width = width;
            Height = height;
        }

        public T this[int x, int y]
        {
            get
            {
                return _list[x + Width * y];
            }
            set
            {
                _list[x + Width * y] = value;
            }
        }
    }
}
