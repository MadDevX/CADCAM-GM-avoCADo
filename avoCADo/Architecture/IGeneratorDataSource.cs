using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo
{
    public interface ITorusGeneratorDataSource
    {
        event Action DataChanged;
        IMeshGenerator Torus { get; set; }
    }
}
