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

        public Vector3 Position => _transform.position;

        public Vector2 ScreenPosition
        {
            get
            {
                return _transform.ScreenCoords(_camera);
            }
        }
        public bool CursorMode { get; set; } = false;

        private readonly GLControl _control;
        private readonly TextBlock _label;
        private readonly IRenderer _gizmoRenderer;
        private readonly ILoop _loop;
        private readonly Camera _camera;
        private readonly Transform _transform;

        private Vector3 _mults = Vector3.Zero;
        private TransformationType _trType = TransformationType.None;

        private Point _prevPos;
        private float _rotateSensitivity = 5.0f;
        private float _translateSensitivity = 5.0f;
        private float _scaleSensitivity = 5.0f;

        public Cursor3D(GLControl control, TextBlock label, Shader shader, ILoop loop, Camera camera)
        {
            _control = control;
            _label = label;
            _loop = loop;
            _camera = camera;
            _transform = new Transform(Vector3.Zero, Vector3.Zero, Vector3.One * 0.2f);
            _gizmoRenderer = new GizmoRenderer(shader);
            Initialize();
        }

        private void Initialize()
        {
            _control.KeyDown += KeyDown;
            _control.MouseMove += MouseMove;
            _loop.OnRenderLoop += OnRender;
            _loop.OnUpdateLoop += OnUpdate;
            UpdateLabel();
        }

        private void OnUpdate(float deltaTime)
        {
            UpdateLabel();
        }

        private void OnRender()
        {
            _gizmoRenderer.Render(_transform, _camera, Matrix4.Identity);
        }

        public void Dispose()
        {
            _control.KeyDown -= KeyDown;
            _control.MouseMove -= MouseMove;
            _loop.OnRenderLoop -= OnRender;
            _loop.OnUpdateLoop -= OnUpdate;
        }

        private void UpdateLabel()
        {
            string axis;
            if (_mults.X > 0.0f) axis = "X";
            else if (_mults.Y > 0.0f) axis = "Y";
            else if (_mults.Z > 0.0f) axis = "Z";
            else axis = "None";
            _label.Text = $"Cursor:\n" +
                          $"World position: {Position}\n" +
                          $"Screen position: ({ScreenPosition.X.ToString("0.#####")}; {ScreenPosition.Y.ToString("0.#####")})\n\n" +
                          $"Transformation:\n" +
                          $"Type: {_trType}\n" +
                          $"Axis: {axis}";
        }

        private void KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if(NodeSelection.Manager.MainSelection != null)
            {
                if (e.KeyCode == System.Windows.Forms.Keys.T) _trType = TransformationType.Translation;
                else if (e.KeyCode == System.Windows.Forms.Keys.R) _trType = TransformationType.Rotation;
                else if (e.KeyCode == System.Windows.Forms.Keys.S) _trType = TransformationType.Scale;
                else if (e.KeyCode == System.Windows.Forms.Keys.Escape) { _trType = TransformationType.None; _mults = Vector3.Zero; }

                if (e.KeyCode == System.Windows.Forms.Keys.F)
                {
                    _transform.position = CalculateCenter();
                }
            }
            else
            {
                if (e.KeyCode == System.Windows.Forms.Keys.F) { _transform.position = _camera.Target; }
                _trType = TransformationType.None;
                _mults = Vector3.Zero;
            }
            if(_trType != TransformationType.None)
            {
                if (e.KeyCode == System.Windows.Forms.Keys.X) _mults = Vector3.UnitX;
                else if (e.KeyCode == System.Windows.Forms.Keys.Y) _mults = Vector3.UnitY;
                else if (e.KeyCode == System.Windows.Forms.Keys.Z) _mults = Vector3.UnitZ;
            }
            if (e.KeyCode == System.Windows.Forms.Keys.G) _camera.Move(Position);
            UpdateLabel();
        }

        private Vector3 CalculateCenter()
        {
            var selectedNodes = NodeSelection.Manager.SelectedNodes;
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

        private void MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Point posDiff = new Point(0, 0);
            if(_trType != TransformationType.None)
            {
                posDiff = new Point(e.Location.X - _prevPos.X, e.Location.Y - _prevPos.Y);
                if(Math.Abs(posDiff.X) > 100 || Math.Abs(posDiff.Y) > 100)
                {
                    posDiff = Point.Empty;
                }
                if(NodeSelection.Manager.MainSelection == null)
                {
                    _trType = TransformationType.None;
                    _mults = Vector3.Zero;
                    UpdateLabel();
                }
            }
            Vector3 delta = ScreenSelectionManager.PixelToNDC(posDiff, _control);
            Vector3 diffVector = new Vector3(_mults.X * posDiff.X, _mults.Y * posDiff.X, _mults.Z * posDiff.X);
            switch (_trType)
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
                        if (CursorMode)
                        {
                            obj.Transform.RotateAround(Position, diffVector * (_rotateSensitivity / _control.Width));
                        }
                        else
                        {
                            obj.Transform.RotateAround(obj.Transform.position, diffVector * (_rotateSensitivity / _control.Width));
                        }
                    }
                    break;
                case TransformationType.Scale:
                    foreach(var obj in NodeSelection.Manager.SelectedNodes)
                    {
                        if(CursorMode)
                        {
                            obj.Transform.ScaleAround(Position, diffVector * (_scaleSensitivity / _control.Width));
                        }
                        else
                        {
                            obj.Transform.scale += diffVector * (_scaleSensitivity / _control.Width);
                        }
                    }
                    break;
                default:
                    break;
            }
            _prevPos = e.Location;
        }
    }
}
