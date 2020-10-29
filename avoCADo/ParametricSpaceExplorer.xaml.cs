using avoCADo.Constants;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace avoCADo
{

    /// <summary>
    /// Interaction logic for ParametricSpaceExplorer.xaml
    /// </summary>
    public partial class ParametricSpaceExplorer : Window
    {
        class DualGLContext : IDisposable
        {
            private const int _viewportMargin = 15;

            public BackgroundManager BackgroundManager { get; }
            public ShaderProvider ShaderProvider { get; }
            public GLControl Control { get; }
            public ViewportManager ViewportManager { get; }
            public ScreenBufferManager ScreenBufferManager { get; }

            public DualGLContext(WindowsFormsHost host)
            {
                Control = CreateGLControl(host);
                BackgroundManager = new BackgroundManager(Color4.Black);
                ShaderProvider = new ShaderProvider();
                ViewportManager = new ViewportManager(Control);
                ScreenBufferManager = new ScreenBufferManager(ViewportManager, BackgroundManager, Control);
            }

            public void MakeCurrent(bool left)
            {
                Control.MakeCurrent();
                System.Drawing.Point pt;
                if (left) pt = System.Drawing.Point.Empty;
                else pt = new System.Drawing.Point(Control.Size.Width / 2 + _viewportMargin, 0);
                ViewportManager.SetCustomViewport(pt, new System.Drawing.Size(Control.Size.Width / 2 - _viewportMargin, Control.Size.Height));
            }

            private GLControl CreateGLControl(WindowsFormsHost host)
            {
                GLControl glControl = new GLControl(new OpenTK.Graphics.GraphicsMode(32, 24, 8, 8));
                glControl.Dock = DockStyle.Fill;
                host.Child = glControl;
                glControl.MakeCurrent();
                return glControl;
            }

            public void Dispose()
            {
                if (Control.Context.IsCurrent)
                {
                    Control.Context.MakeCurrent(null);
                }
                ViewportManager.Dispose();
                Control.Dispose();
                ShaderProvider.Dispose();
            }
        }

        private DualGLContext _glContext;
        private Color4 _renderColor = Color4.Red;

        private List<ParametricObjectRenderer> _rendPList = new List<ParametricObjectRenderer>();
        private List<ParametricObjectRenderer> _rendQList = new List<ParametricObjectRenderer>();

        private DummyCamera _cam = new DummyCamera();
        private DummyNode _node = new DummyNode();

        public ParametricSpaceExplorer()
        {
            InitializeComponent();

            _glContext = new DualGLContext(host);
            _glContext.Control.Paint += Control_Paint;

            this.Closed += DisposeResources;
        }

        private void Control_Paint(object sender, PaintEventArgs e) => Render();

        private void OnTick(object sender, EventArgs e)
        {
            Render();
        }

        private void DisposeResources(object sender, EventArgs e)
        {
            this.Closed -= DisposeResources;
            CompositionTarget.Rendering -= OnTick;

            _glContext.Control.Paint -= Control_Paint;
            foreach (var rend in _rendPList)
            {
                rend.Dispose();
            }
            foreach (var rend in _rendQList)
            {
                rend.Dispose();
            }
            _glContext.Dispose();
        }

        public void Show(INode pNode, INode qNode)
        {
            var pGen = (pNode.GetComponent<Renderer>()?.GetGenerator() as ISurfaceGenerator);
            var qGen = (qNode.GetComponent<Renderer>()?.GetGenerator() as ISurfaceGenerator);
            if (pGen == null || qGen == null) throw new InvalidOperationException("Provided nodes do not represent valid surfaces");
            Initialize(pGen.Surface, qGen.Surface);
            SetLabels(pNode, qNode, pGen.Surface, qGen.Surface);

            CompositionTarget.Rendering += OnTick;
            base.ShowDialog();
        }

        private void SetLabels(INode pNode, INode qNode, ISurface p, ISurface q)
        {
            pLabel.Text = $"P surface [ {pNode.Name} ]";
            qLabel.Text = $"Q surface [ {qNode.Name} ]";
            pUMinVal.Text = $"U={p.ParameterURange.X}";
            pUMaxVal.Text = $"U={p.ParameterURange.Y}";
            pVMinVal.Text = $"V={p.ParameterVRange.X}";
            pVMaxVal.Text = $"V={p.ParameterVRange.Y}";
            qUMinVal.Text = $"U={q.ParameterURange.X}";
            qUMaxVal.Text = $"U={q.ParameterURange.Y}";
            qVMinVal.Text = $"V={q.ParameterVRange.X}";
            qVMaxVal.Text = $"V={q.ParameterVRange.Y}";
        }

        private void Initialize(ISurface p, ISurface q)
        {
            ParametricSpaceConverter.SetupData(p, q, _rendPList, _glContext.ShaderProvider, _renderColor);
            if (p == q)
            {
                ParametricSpaceConverter.SetupData(q, p, _rendQList, _glContext.ShaderProvider, _renderColor, true);
            }
            else
            {
                ParametricSpaceConverter.SetupData(q, p, _rendQList, _glContext.ShaderProvider, _renderColor);
            }
        }

        private void Render()
        {
            _glContext.BackgroundManager.BackgroundColor = Color4.White;
            _glContext.ScreenBufferManager.ResetScreenBuffer();
            _glContext.BackgroundManager.BackgroundColor = Color4.Black;
            Render(_glContext, _rendPList, true);
            Render(_glContext, _rendQList, false);
            _glContext.Control.SwapBuffers();
        }

        private void Render(DualGLContext context, List<ParametricObjectRenderer> rendList, bool left)
        {
            context.MakeCurrent(left);
            context.ScreenBufferManager.ResetScreenBuffer(true);
            context.ShaderProvider.UpdateShadersCameraMatrices(_cam);
            foreach (var rend in rendList)
            {
                rend.Render(_cam, Matrix4.Identity, Matrix4.Identity);
            }
        }
    }
}
