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
    public class ScreenSelectionHandler : IDisposable
    {
        private readonly GLControl _control;
        private readonly Camera _camera;
        private readonly Scene _scene;
        private readonly SelectionManager _selectionManager;
        private readonly float _selectionThreshold;
        private bool _ignoreClick = false;

        public ScreenSelectionHandler(GLControl control, Camera camera, Scene scene, float selectionThreshold = 0.2f)
        {
            _control = control;
            _camera = camera;
            _scene = scene;
            _selectionManager = NodeSelection.Manager;
            _selectionThreshold = selectionThreshold;
            Initialize();
        }

        private void Initialize()
        {
            _control.MouseDown += SelectionOnMouseDown;
            _control.GotFocus += IgnoreFirstClick;
        }

        public void Dispose()
        {
            _control.MouseDown -= SelectionOnMouseDown;
            _control.GotFocus -= IgnoreFirstClick;
        }

        private void IgnoreFirstClick(object sender, EventArgs e)
        {
            _ignoreClick = true;
        }

        private void SelectionOnMouseDown(object sender, MouseEventArgs e)
        {
            if (_ignoreClick) { _ignoreClick = false; return; }
            if (e.Button == MouseButtons.Left && System.Windows.Input.Keyboard.Modifiers != System.Windows.Input.ModifierKeys.Alt)
            {
                var node = Select(e.Location);
                if (System.Windows.Input.Keyboard.Modifiers == System.Windows.Input.ModifierKeys.Shift)
                {
                    if (node != null)
                    {
                        if (node is VirtualNode || _selectionManager.MainSelection is VirtualNode)
                        {
                            _selectionManager.Select(node);
                        }
                        else
                        {
                            _selectionManager.ToggleSelection(node);
                        }
                    }
                }
                else
                {
                    if (node != null)
                    {
                        _selectionManager.Select(node);
                    }
                    else
                    {
                        _selectionManager.ResetSelection();
                    }
                }
            }
        }

        private INode Select(Point location)
        {
            float curDist = float.MaxValue;
            INode curSelect = null;
            var mousePos = PixelToNDC(location, _control);

            TraverseCollection(_scene.Children, mousePos, ref curDist, ref curSelect);
            TraverseCollection(_scene.VirtualChildren, mousePos, ref curDist, ref curSelect);
            return curSelect;
        }

        private void TraverseCollection(IList<INode> list, Vector3 mousePos, ref float curDist, ref INode curSelect)
        {
            foreach (var node in list)
            {
                CheckSelection(node, mousePos, ref curDist, ref curSelect);
                foreach (var child in node.Children)
                {
                    CheckSelection(child, mousePos, ref curDist, ref curSelect);
                }
            }
        }

        private void CheckSelection(INode node, Vector3 mousePosition, ref float curDist, ref INode curSelect)
        {
            var dist = CheckDistanceFromScreenCoords(_camera, mousePosition, node);
            if (dist <= _selectionThreshold && dist < curDist)
            {
                curDist = dist;
                curSelect = node;
            }
        }
        private float CheckDistanceFromScreenCoords(Camera camera, Vector3 mousePosition, INode node)
        {
            var screenSpace = node.Transform.ScreenCoords(camera);
            Vector2 diff = new Vector2(mousePosition.X - screenSpace.X, mousePosition.Y - screenSpace.Y);
            return diff.Length;
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
