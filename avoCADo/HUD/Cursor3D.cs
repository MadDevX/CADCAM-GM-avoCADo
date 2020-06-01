using OpenTK;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;

namespace avoCADo
{
    public class Cursor3D : IDisposable
    {
        public Vector3 Position => Transform.Position;

        public Vector2 ScreenPositionNDC
        {
            get
            {
                return Transform.ScreenCoords(_camera);
            }
        }

        public Point ScreenPositionPixels
        {
            get
            {
                var screenPos = ScreenPositionNDC;
                screenPos.X += 1.0f;
                screenPos.Y += 1.0f;
                screenPos *= 0.5f;
                return new Point((int)(_control.Width * screenPos.X), (int)(_control.Height * (1.0f - screenPos.Y)));
            }
        }

        private readonly GLControl _control;
        private readonly IRenderer _gizmoRenderer;
        private readonly IRenderLoop _renderLoop;
        private readonly Camera _camera;
        private readonly SelectionManager _selectionManager;
        public Transform Transform { get; }

        public Cursor3D(GLControl control, ShaderWrapper shader, IRenderLoop renderLoop, Camera camera)
        {
            _control = control;
            _renderLoop = renderLoop;
            _camera = camera;
            _selectionManager = NodeSelection.Manager;
            Transform = new Transform(Vector3.Zero, Vector3.Zero, Vector3.One * 0.2f);
            _gizmoRenderer = new GizmoRenderer(shader);
            Initialize();
        }

        private void Initialize()
        {
            _control.KeyDown += KeyDown;
            _renderLoop.OnRenderLoop += OnRender;
        }

        public void Dispose()
        {
            _control.KeyDown -= KeyDown;
            _renderLoop.OnRenderLoop -= OnRender;
        }

        private void KeyDown(object sender, KeyEventArgs e)
        {
            if (_selectionManager.MainSelection != null)
            {
                if (e.KeyCode == System.Windows.Forms.Keys.F)
                {
                    if (System.Windows.Input.Keyboard.Modifiers == System.Windows.Input.ModifierKeys.Shift)
                    {
                        Transform.Position = _camera.Target;
                    }
                    else
                    {
                        Transform.Position = CalculateCenter();
                    }
                }
            }
            else
            {
                if (e.KeyCode == System.Windows.Forms.Keys.F) { Transform.Position = _camera.Target; }
            }
            if (e.KeyCode == System.Windows.Forms.Keys.G) _camera.Move(Position);
        }

        private void OnRender()
        {
            _gizmoRenderer.Render(_camera, Transform.LocalModelMatrix, Matrix4.Identity);
        }

        private Vector3 CalculateCenter()
        {
            var selectedNodes = _selectionManager.SelectedNodes;
            if(selectedNodes.Count == 0)
            {
                return Vector3.Zero;
            }
            else
            {
                var res = Vector3.Zero;
                foreach(var obj in selectedNodes)
                {
                    res += obj.Transform.WorldPosition;
                }
                res /= selectedNodes.Count;
                return res;
            }
        }
    }
}
