using OpenTK;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace avoCADo
{
    public class CameraMovement : IDisposable
    {
        private Camera _camera;
        private GLControl _control;

        private Point _prevMousePosition;
        private bool _posInitialized = false;


        public CameraMovement(Camera camera, GLControl control)
        {
            _camera = camera;
            _control = control;

            Init();
        }

        private void Init()
        {
            _control.MouseMove += OnMouseMove;
            _control.MouseWheel += OnMouseWheel;
            _control.MouseDoubleClick += OnDoubleClick;
        }

        public void Dispose()
        {
            _control.MouseMove -= OnMouseMove;
            _control.MouseWheel -= OnMouseWheel;
            _control.MouseDoubleClick -= OnDoubleClick;
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (_posInitialized == false) { _prevMousePosition = e.Location; _posInitialized = true; }
            var diff = GetPositionDifference(e);
            if(e.Button == MouseButtons.Left && Control.ModifierKeys == Keys.Alt)
            {
                _camera.Rotate(diff.X, diff.Y);
            }
            else if(e.Button == MouseButtons.Middle)
            {
                _camera.Translate(diff.X, diff.Y);
            }
        }

        private void OnMouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta != 0)
            {
                _camera.ChangeOffset(e.Delta);
            }
        }

        private void OnDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle)
            {
                _camera.Reset();
            }
        }

        private Vector2 GetPositionDifference(MouseEventArgs e)
        {
            var curPos = e.Location;
            var diff = new Point(curPos.X - _prevMousePosition.X, curPos.Y - _prevMousePosition.Y);
            var relative = new Vector2((float)diff.X / _control.Width, (float)diff.Y / _control.Height);
            _prevMousePosition = e.Location;
            return relative;
        }
    }
}
