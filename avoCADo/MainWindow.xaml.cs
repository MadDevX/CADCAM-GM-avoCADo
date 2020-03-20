using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Drawing;
using System.Windows;
using OpenTK.Graphics.OpenGL;

using Size = System.Drawing.Size;
using Color = System.Drawing.Color;
using System.Windows.Threading;
using System.Diagnostics;
using System.Windows.Media;

namespace avoCADo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, ILoop, ITorusGeneratorDataSource
    {
        public event Action DataChanged;

        private IMeshGenerator _torus;
        public IMeshGenerator Torus
        {
            get => _torus;
            set
            {
                _torus = value;
                DataChanged?.Invoke();
            }
        }


        private GLControl _glControl;
        private ScreenBufferManager _screenBufferManager;
        private ViewportManager _viewportManager;

        private Shader _shader;
        private Scene _scene;
        private Camera _camera;
        private CameraMovement _camMovement;

        private DispatcherTimer _timer;

        private Stopwatch _stopwatch;
        private Stopwatch _deltaStopwatch;

        private int _frames = 0;
        private Node _parent;
        private Node _child;
        private Node _point;

        public event Action<float> OnUpdate;
        private TransformHandler _transformHandler;

        public MainWindow()
        {
            InitializeComponent();
            _stopwatch = new Stopwatch();
            _deltaStopwatch = new Stopwatch();
            _glControl = new GLControl();

            _glControl.Paint += GLControlOnPaint;
            _glControl.Dock = DockStyle.Fill;
            Host.Child = _glControl;

            _glControl.MakeCurrent();
            _screenBufferManager = new ScreenBufferManager(Color.FromArgb(255, Color.LightBlue));
            _viewportManager = new ViewportManager(_glControl);
            _shader = new Shader("vs.vert", "fs.frag");
            _scene = new Scene("Main");
            _camera = new Camera(_viewportManager);
            _camMovement = new CameraMovement(_camera, _glControl);

            _parent = new Node(new Transform(Vector3.Zero, new Vector3(0.0f, 0.0f, MathHelper.DegreesToRadians(0.0f)), Vector3.One), new MeshRenderer(_shader, new TorusGenerator(0.5f, 0.2f, 30, 30)), "parent torus");
            _child = new Node(new Transform(Vector3.UnitX, new Vector3(0.0f, 0.0f, MathHelper.DegreesToRadians(45.0f)), Vector3.One * 0.5f), new MeshRenderer(_shader, new TorusGenerator(0.5f, 0.2f, 30, 30)), "child torus");
            _point = new Node(new Transform(Vector3.UnitX, Vector3.Zero, Vector3.One), new PointRenderer(_shader), "point");
            _parent.AttachChild(_child);
            _scene.AttachChild(_parent);
            _scene.AttachChild(new Node(new Transform(-Vector3.UnitX, new Vector3(0.0f, 0.0f, MathHelper.DegreesToRadians(45.0f)), Vector3.One * 0.5f), new MeshRenderer(_shader, new TorusGenerator(0.5f, 0.2f, 20, 20)), "child torus"));
            _scene.AttachChild(new Node(new Transform(Vector3.UnitY, new Vector3(0.0f, 0.0f, MathHelper.DegreesToRadians(45.0f)), Vector3.One * 0.5f), new MeshRenderer(_shader, new TorusGenerator(0.5f, 0.2f, 10, 10)), "child torus"));
            _scene.AttachChild(new Node(new Transform(-Vector3.UnitY, new Vector3(0.0f, 0.0f, MathHelper.DegreesToRadians(45.0f)), Vector3.One * 0.5f), new MeshRenderer(_shader, new TorusGenerator(0.5f, 0.2f, 5, 5)), "child torus"));
            _scene.AttachChild(_point);

            hierarchy.treeView.Items.Add(_scene);
            transformView.Transform = _parent.Transform;
            InitLoop();
            BindControls();
            _transformHandler = new TransformHandler(transformView, this, this);
        }

        private void InitLoop()
        {
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(8);
            _timer.Tick += SetDirty;
            _timer.Start();

            _deltaStopwatch.Start();
            CompositionTarget.Rendering += OnTick;
        }

        private void SetDirty(object sender, EventArgs e)
        {
            Host.InvalidateVisual();
        }

        private void OnTick(object sender, EventArgs e)
        {
            var deltaTime = (float)_deltaStopwatch.Elapsed.TotalSeconds;
            _deltaStopwatch.Restart();
            //var rot = _parent.Transform.Rotation;//.Y += 0.1f;// = Quaternion.FromEulerAngles(0.0f, 0.01f, 0.0f) *_parent.Transform.rotation;
            //rot.Y += 0.5f * deltaTime;
            //_parent.Transform.Rotation = rot;
            //rot = _child.Transform.Rotation;
            //rot.Z += 0.5f * deltaTime;// = Quaternion.FromEulerAngles(0.0f, 0.0f, 0.01f) * _child.Transform.rotation;
            //_child.Transform.Rotation = rot;
            OnUpdate?.Invoke(deltaTime);
            _glControl.Invalidate();
        }

        private void GLControlOnPaint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            #region Timer
            _stopwatch.Start();
            #endregion

            _glControl.MakeCurrent();
            _screenBufferManager.ResetScreenBuffer();
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            float halfWidth = _glControl.Width / 2.0f;
            float halfHeight = _glControl.Height / 2.0f;
            GL.Ortho(-halfWidth, halfWidth, halfHeight, -halfHeight, 1000.0f, -1000.0f);
            GL.Viewport(_glControl.Size);

            _scene.Render(_camera);

            GL.Finish();

            _glControl.SwapBuffers();

            #region Timer
            _frames++;
            if (_stopwatch.ElapsedTicks >= Stopwatch.Frequency)
            {
                this.Title = $"{_frames} FPS";
                _frames = 0;
                _stopwatch.Reset();
            }
            #endregion
        }



        protected override void OnClosed(EventArgs e)
        {
            CompositionTarget.Rendering -= OnTick;
            _shader.Dispose();
            _scene.Dispose();
            _viewportManager.Dispose();
            _camMovement.Dispose();
            _camera.Dispose();
            _timer.Tick -= SetDirty;
            _timer.Stop();
            UnbindControls();
            _transformHandler.Dispose();
            base.OnClosed(e);
        }
    }
}
