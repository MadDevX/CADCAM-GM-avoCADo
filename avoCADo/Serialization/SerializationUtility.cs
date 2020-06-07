using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo.Serialization
{
    public static class SerializationUtility
    {
        public static WrapType ConvertToWrapType(WrapMode mode)
        {
            if (mode == WrapMode.None) return WrapType.None;
            if (mode == WrapMode.Column) return WrapType.Column;
            if (mode == WrapMode.Row) return WrapType.Row;
            return WrapType.None;
        }
        public static WrapMode ConvertToWrapMode(WrapType mode)
        {
            if (mode == WrapType.None) return WrapMode.None;
            if (mode == WrapType.Column) return WrapMode.Column;
            if (mode == WrapType.Row) return WrapMode.Row;
            return WrapMode.None;
        }
    }
}
