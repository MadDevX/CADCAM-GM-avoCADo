using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo
{
    public class CompositionRoot : IDisposable
    {
        private MainWindow _window;
        private GLControl _control;

        public CompositionRoot(GLControl control, MainWindow window)
        {
            _window = window;
            _control = control;
            Initialize();
        }

        private void Initialize()
        {
        }
        public void Dispose()
        {
        }
    }
}
