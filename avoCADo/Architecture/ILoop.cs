using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo
{
    public interface ILoop
    {
        event Action<float> OnUpdateLoop;
        event Action OnRenderLoop;
    }
}
