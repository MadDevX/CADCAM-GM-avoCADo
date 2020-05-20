using avoCADo.HUD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace avoCADo
{
    public class LabelBindingRefresher : IDisposable
    {
        private readonly IBindingUpdatable[] _updatables;
        private readonly IUpdateLoop _loop;

        public LabelBindingRefresher(IUpdateLoop loop, params IBindingUpdatable[] updatables)
        {
            _updatables = updatables;
            _loop = loop;
            Initialize();
        }

        private void Initialize()
        {
            _loop.OnUpdateLoop += UpdateCursorInfo;
        }

        public void Dispose()
        {
            _loop.OnUpdateLoop -= UpdateCursorInfo;
        }

        private void UpdateCursorInfo(float obj)
        {
            foreach(var updatable in _updatables)
            {
                updatable.UpdateBindings();
            }
        }
    }
}
