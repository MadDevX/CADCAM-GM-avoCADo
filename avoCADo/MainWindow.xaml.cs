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
        private Cursor3D _cursor;

        private DispatcherTimer _timer;

        private Stopwatch _stopwatch;
        private Stopwatch _deltaStopwatch;

        private int _frames = 0;

        public event Action<float> OnUpdateLoop;
        public event Action OnRenderLoop;

        private TransformHandler _transformHandler;
        private ScreenSelectionManager _selectionManager;

        private float _totalTime;

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
            _screenBufferManager = new ScreenBufferManager(Color.FromArgb(255, Color.FromArgb(40,40,40)));
            _viewportManager = new ViewportManager(_glControl);
            _shader = new Shader("vs.vert", "fs.frag");
            _scene = new Scene("Main");
            _camera = new Camera(_viewportManager);
            _camMovement = new CameraMovement(_camera, _glControl);

            TestSceneInitializer.SpawnTestObjects(_scene, _shader);
            hierarchy.treeView.Items.Add(_scene);
            InitLoop();
            BindControls();
            _selectionManager = new ScreenSelectionManager(_glControl, _camera, _scene);
            _cursor = new Cursor3D(_glControl, transformationsLabel, _shader, this, _camera);
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
            _totalTime += deltaTime;
            OnUpdateLoop?.Invoke(deltaTime);
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

            OnRenderLoop?.Invoke();
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
            _glControl.Paint -= GLControlOnPaint;
            _shader.Dispose();
            _scene.Dispose();
            _viewportManager.Dispose();
            _camMovement.Dispose();
            _camera.Dispose();
            _timer.Tick -= SetDirty;
            _timer.Stop();
            UnbindControls();
            _selectionManager.Dispose();
            _cursor.Dispose();
            _transformHandler.Dispose();
            base.OnClosed(e);
        }

        private void LocalMode_Checked(object sender, RoutedEventArgs e)
        {
            if (_cursor != null)
                _cursor.CursorMode = false;
        }

        private void CursorMode_Checked(object sender, RoutedEventArgs e)
        {
            if (_cursor != null)
                _cursor.CursorMode = true;
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if(e.Key == Key.System && e.OriginalSource is System.Windows.Forms.Integration.WindowsFormsHost)
            {
                e.Handled = true;
            }
        }
    }
}
