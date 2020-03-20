using OpenTK;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace avoCADo
{
    public class Cursor3D : IDisposable
    {
        enum TransformationType
        {
            None,
            Translation,
            Rotation,
            Scale
        };

        public Vector3 Position;

        private readonly GLControl _control;
        private readonly TextBlock _label;

        private Vector3 _mults = Vector3.Zero;
        private TransformationType _trType = TransformationType.None;

        private Point _prevPos;
        private float _rotateSensitivity = 5.0f;
        private float _translateSensitivity = 5.0f;

        public Cursor3D(GLControl control, TextBlock label)
        {
            _control = control;
            _label = label;
            Initialize();
        }

        private void Initialize()
        {
            _control.KeyDown += KeyDown;
            _control.MouseMove += MouseMove;
            UpdateLabel();
        }

        public void Dispose()
        {
            _control.KeyDown -= KeyDown;
            _control.MouseMove -= MouseMove;
        }

        private void UpdateLabel()
        {
            string axis;
            if (_mults.X > 0.0f) axis = "X";
            else if (_mults.Y > 0.0f) axis = "Y";
            else if (_mults.Z > 0.0f) axis = "Z";
            else axis = "None";
            _label.Text = $"Current transformation: {_trType}\n" +
                          $"Axis: {axis}";
        }

        private void KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if(NodeSelection.Manager.MainSelection != null)
            {
                if (e.KeyCode == System.Windows.Forms.Keys.T) _trType = TransformationType.Translation;
                else if (e.KeyCode == System.Windows.Forms.Keys.R) _trType = TransformationType.Rotation;
                else if (e.KeyCode == System.Windows.Forms.Keys.Escape) { _trType = TransformationType.None; _mults = Vector3.Zero; }
            }
            else
            {
                _trType = TransformationType.None;
                _mults = Vector3.Zero;
            }
            if(_trType != TransformationType.None)
            {
                if (e.KeyCode == System.Windows.Forms.Keys.X) _mults = Vector3.UnitX;
                else if (e.KeyCode == System.Windows.Forms.Keys.Y) _mults = Vector3.UnitY;
                else if (e.KeyCode == System.Windows.Forms.Keys.Z) _mults = Vector3.UnitZ;
            }
            UpdateLabel();
        }

        private void MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Point posDiff = new Point(0, 0);
            if(_trType != TransformationType.None)
            {
                posDiff = new Point(e.Location.X - _prevPos.X, e.Location.Y - _prevPos.Y);
            }
            Vector3 delta = ScreenSelectionManager.PixelToNDC(posDiff, _control);
            switch(_trType)
            {
                case TransformationType.Translation:
                    foreach (var obj in NodeSelection.Manager.SelectedNodes)
                    {
                        obj.Transform.Translate(new Vector3(_mults.X * posDiff.X, _mults.Y * posDiff.X, _mults.Z * posDiff.X) * (_translateSensitivity / _control.Width));
                    }
                    break;
                case TransformationType.Rotation:
                    foreach(var obj in NodeSelection.Manager.SelectedNodes)
                    {
                        obj.Transform.RotateAround(Position, new Vector3(_mults.X * posDiff.X, _mults.Y * posDiff.X, _mults.Z * posDiff.X) * (_rotateSensitivity / _control.Width));
                    }
                    break;
                default:
                    break;
            }
            _prevPos = e.Location;
        }
    }
}
