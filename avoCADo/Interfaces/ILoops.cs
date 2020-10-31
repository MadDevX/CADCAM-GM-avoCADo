using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo
{
    public interface IUpdateLoop
    {
        event Action<float> OnUpdateLoop;
        event Action<float> OnLateUpdateLoop;
    }
    public interface IRenderLoop
    {
        event Action OnRenderLoop;
    }
}
