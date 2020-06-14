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

        public int DataWidth { get; private set; }
        public int DataHeight { get; private set; }

        public int Count => Width * Height;
        public int DataCount => DataWidth * DataHeight;
        
        private List<T> _list;
        public IList<T> RawData => _list;

        public CoordList()
        {
            _list = new List<T>();
            Width = Height = DataWidth = DataHeight = 0;
        }

        public CoordList(IList<T> list, int width, int height)
        {
            _list = new List<T>(width * height);
            SetData(list, width, height);
        }

        /// <summary>
        /// Clears data and initializes table of given dimensions filled with nulls
        /// </summary>
        /// <param name="dataWidth"></param>
        /// <param name="dataHeight"></param>
        public void ResetSize(int dataWidth, int dataHeight)
        {
            DataWidth = Width = dataWidth;
            DataHeight = Height = dataHeight;
            _list.Clear();
            for (int i = 0; i < dataWidth * dataHeight; i++)
            {
                _list.Add(default);
            }
        }

        public void SetData(IList<T> list, int dataWidth, int dataHeight, int width, int height)
        {
            SetData(list, dataWidth, dataHeight);
            Width = width;
            Height = height;
        }

        public void SetData(IList<T> list, int dataWidth, int dataHeight)
        {
            if (list.Count != dataWidth * dataHeight)
            {
                throw new InvalidOperationException("Provided list does not match provided dimensions");
            }
            _list.Clear();
            _list.AddRange(list);
            Width = DataWidth = dataWidth;
            Height = DataHeight = dataHeight;
        }


        public T this[int x, int y]
        {
            get
            {
                return _list[GetCoord(x, y)];
            }
            set
            {
                _list[GetCoord(x, y)] = value;
            }
        }

        private int GetCoord(int x, int y)
        {
            x %= DataWidth;
            y %= DataHeight;
            return x + DataWidth * y;
        }
    }
}
