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
using avoCADo.HUD;
using avoCADo.Serialization;
using avoCADo.Miscellaneous;

namespace avoCADo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IUpdateLoop
    {
        private NodeFactory _nodeFactory;
        private NodeImporter _nodeImporter;
        private NodeExporter _nodeExporter;
        private TransformationsManager _transformationsManager;
        private SceneManager _sceneManager;
        private Cursor3D _cursor3D;
        private RenderLoop _renderLoop;
        private IInstructionBuffer _instructionBuffer;

        private GLControl _glControl;
        private CompositionRoot _compositionRoot;
        private DispatcherTimer _timer;
        private Stopwatch _deltaStopwatch;

        public event Action<float> OnUpdateLoop;

        public MainWindow()
        {
            InitializeComponent();

            CreateGLControl();
            _compositionRoot = new CompositionRoot(_glControl, this);

            InitLoop();
        }

        public void Initialize(NodeFactory nodeFactory, 
                               TransformationsManager transformationsManager, 
                               NodeImporter nodeImporter,
                               NodeExporter nodeExporter,
                               SceneManager sceneManager,
                               Cursor3D cursor3D,
                               RenderLoop renderLoop,
                               IInstructionBuffer instructionBuffer)
        {
            _nodeFactory = nodeFactory;
            _transformationsManager = transformationsManager;
            _nodeImporter = nodeImporter;
            _nodeExporter = nodeExporter;
            _sceneManager = sceneManager;
            _cursor3D = cursor3D;
            _renderLoop = renderLoop;
            _instructionBuffer = instructionBuffer;
        }

        private void CreateGLControl()
        {
            _glControl = new GLControl(new OpenTK.Graphics.GraphicsMode(32, 24, 8, 8));
            _glControl.Dock = DockStyle.Fill;
            Host.Child = _glControl;
            _glControl.MakeCurrent();
        }

        private void InitLoop()
        {
            _deltaStopwatch = new Stopwatch();

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(16);
            _timer.Tick += SetDirty;
            _timer.Start();

            CompositionTarget.Rendering += OnTick;
            _deltaStopwatch.Start();
        }

        private void SetDirty(object sender, EventArgs e)
        {
            Host.InvalidateVisual();
        }

        private void OnTick(object sender, EventArgs e)
        {
            var deltaTime = (float)_deltaStopwatch.Elapsed.TotalSeconds;
            _deltaStopwatch.Restart();
            OnUpdateLoop?.Invoke(deltaTime);
            _glControl.Invalidate();
        }

        protected override void OnClosed(EventArgs e)
        {
            CompositionTarget.Rendering -= OnTick;
            _compositionRoot.Dispose();
            _timer.Tick -= SetDirty;
            _timer.Stop();
            base.OnClosed(e);
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
