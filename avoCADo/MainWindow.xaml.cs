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
using System.Windows.Threading;

namespace avoCADo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private GLControl _glControl;
        private ScreenBufferManager _manager;

        private Shader _shader;
        private Scene _scene;

        public MainWindow()
        {
            InitializeComponent();

            _glControl = new GLControl();

            _glControl.Paint += GLControlOnPaint;
            _glControl.Dock = DockStyle.Fill;
            Host.Child = _glControl;

            _glControl.MakeCurrent();
            _manager = new ScreenBufferManager(Color.FromArgb(255, Color.LightBlue));
            _shader = new Shader("vs.vert", "fs.frag");
            _scene = new Scene();
            _scene.AddNode(new Node(new Transform(), new Renderer(_shader)));

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(16);
            timer.Tick += OnTick;
            timer.Start();

        }

        private void OnTick(object sender, EventArgs e)
        {
            _glControl.Invalidate();
        }

        private void GLControlOnPaint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            _glControl.MakeCurrent();
            _manager.ResetScreenBuffer();
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            float halfWidth = _glControl.Width/2.0f;
            float halfHeight = _glControl.Height/2.0f;
            GL.Ortho(-halfWidth, halfWidth, halfHeight, -halfHeight, 1000.0f, -1000.0f);
            GL.Viewport(_glControl.Size);
            _scene.Render();

            GL.Finish();

            _glControl.SwapBuffers();
        }

        protected override void OnClosed(EventArgs e)
        {
            _shader.Dispose();
            _scene.Dispose();
            base.OnClosed(e);
        }
    }
}
