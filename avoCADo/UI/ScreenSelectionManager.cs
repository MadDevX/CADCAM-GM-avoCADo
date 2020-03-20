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
    public class ScreenSelectionManager : IDisposable
    {
        private readonly GLControl _control;
        private readonly Camera _camera;
        private readonly Scene _scene;
        private readonly float _selectionThreshold;

        public ScreenSelectionManager(GLControl control, Camera camera, Scene scene, float selectionThreshold = 0.2f)
        {
            _control = control;
            _camera = camera;
            _scene = scene;
            _selectionThreshold = selectionThreshold;
            Initialize();
        }

        private void Initialize()
        {
            _control.MouseDown += SelectionOnMouseDown;
        }

        public void Dispose()
        {
            _control.MouseDown -= SelectionOnMouseDown;
        }

        private void SelectionOnMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                var node = Select(e.Location);
                if (System.Windows.Input.Keyboard.Modifiers == System.Windows.Input.ModifierKeys.Shift)
                {
                    NodeSelection.Manager.ToggleSelection(node);
                }
                else
                {
                    NodeSelection.Manager.Select(node);
                }
            }
        }

        private Node Select(Point location)
        {
            float curDist = float.MaxValue;
            Node curSelect = null;
            var mousePos = PixelToNDC(location, _control);
            foreach (var obj in _scene.Children)
            {
                CheckSelection(obj, mousePos, ref curDist, ref curSelect);
                foreach (var child in obj.Children)
                {
                    CheckSelection(child, mousePos, ref curDist, ref curSelect);
                }
            }
            return curSelect;
        }

        private void CheckSelection(Node obj, Vector3 mousePosition, ref float curDist, ref Node curSelect)
        {
            var dist = obj.Transform.CheckDistanceFromScreenCoords(_camera, mousePosition);
            if (dist <= _selectionThreshold && dist < curDist)
            {
                curDist = dist;
                curSelect = obj;
            }
        }

        public static Vector3 PixelToNDC(Point location, GLControl ctrl)
        {
            var halfX = ctrl.Width / 2;
            var halfY = ctrl.Height / 2;
            var x =  (float)(location.X - halfX) / halfX;
            var y = -(float)(location.Y - halfY) / halfY;
            return new Vector3(x, y, 0.0f);
        }
    }
}
