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
    public partial class MainWindow : Window
    {
        private GLControl _glControl;
        private ScreenBufferManager _screenBufferManager;
        private ViewportManager _viewportManager;

        private Shader _shader;
        private Scene _scene;
        private Camera _camera;
        private CameraMovement _camMovement;

        private DispatcherTimer _timer;

        private Stopwatch _stopwatch;

        private int _frames = 0;

        private TorusGenerator _torus;
        private Node _parent;
        private Node _child;

        public MainWindow()
        {
            InitializeComponent();
            _stopwatch = new Stopwatch();
            _glControl = new GLControl();

            _glControl.Paint += GLControlOnPaint;
            _glControl.Dock = DockStyle.Fill;
            Host.Child = _glControl;

            _glControl.MakeCurrent();
            _screenBufferManager = new ScreenBufferManager(Color.FromArgb(255, Color.LightBlue));
            _viewportManager = new ViewportManager(_glControl);
            _shader = new Shader("vs.vert", "fs.frag");
            _scene = new Scene();
            _camera = new Camera(_viewportManager);
            _camMovement = new CameraMovement(_camera, _glControl);
            _torus = new TorusGenerator(0.5f, 0.2f, 30, 30);
            _parent = new Node(new Transform(Vector3.Zero, Quaternion.FromEulerAngles(new Vector3(0.0f, 0.0f, MathHelper.DegreesToRadians(0.0f))), Vector3.One), new Renderer(_shader, _torus));
            _child = new Node(new Transform(Vector3.UnitX, Quaternion.FromEulerAngles(new Vector3(0.0f, 0.0f, MathHelper.DegreesToRadians(45.0f))), Vector3.One * 0.5f), new Renderer(_shader, _torus));
            _parent.AttachChild(_child);
            _scene.AddNode(_parent);
            InitLoop();
            BindControls();
        }

        private void InitLoop()
        {
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(8);
            _timer.Tick += SetDirty;
            _timer.Start();

            CompositionTarget.Rendering += OnTick;
        }

        private void SetDirty(object sender, EventArgs e)
        {
            Host.InvalidateVisual();
        }

        private void OnTick(object sender, EventArgs e)
        {
            _parent.transform.rotation = Quaternion.FromEulerAngles(0.0f, 0.01f, 0.0f) * _parent.transform.rotation;
            _child.transform.rotation = Quaternion.FromEulerAngles(0.0f, 0.0f, 0.01f) * _child.transform.rotation;
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
            base.OnClosed(e);
        }
    }
}
