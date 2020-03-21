using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo
{
    public class TransformationModeHandler : IDisposable
    {
        private MainWindow _window;
        private Cursor3D _cursor;

        public TransformationModeHandler(MainWindow window, Cursor3D cursor)
        {
            _window = window;
            _cursor = cursor;
            Initialize();
        }

        private void Initialize()
        {
            _window.radioBtnLocalMode.Checked += RadioBtnLocalMode_Checked;
            _window.radioBtnCursorMode.Checked += RadioBtnCursorMode_Checked;
        }

        public void Dispose()
        {
            _window.radioBtnLocalMode.Checked -= RadioBtnLocalMode_Checked;
            _window.radioBtnCursorMode.Checked -= RadioBtnCursorMode_Checked;
        }

        private void RadioBtnCursorMode_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            if (_cursor != null)
                _cursor.CursorMode = true;
        }

        private void RadioBtnLocalMode_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            if (_cursor != null)
                _cursor.CursorMode = false;
        }
    }
}
