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
            public ShaderProvider ShaderProvider { get; }
            public GLControl Control { get; }
            public ViewportManager ViewportManager { get; }
            public ScreenBufferManager ScreenBufferManager { get; }

            public DualGLContext(WindowsFormsHost host, BackgroundManager bgManager)
            {
                Control = CreateGLControl(host);
                ShaderProvider = new ShaderProvider();
                ViewportManager = new ViewportManager(Control);
                ScreenBufferManager = new ScreenBufferManager(bgManager, Control);
            }

            public void MakeCurrent(bool left)
            {
                Control.MakeCurrent();
                System.Drawing.Point pt;
                if (left) pt = System.Drawing.Point.Empty;
                else pt = new System.Drawing.Point(Control.Size.Width / 2, 0);
                ViewportManager.SetCustomViewport(pt, new System.Drawing.Size(Control.Size.Width / 2, Control.Size.Height));
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

        private ParametricObjectRenderer _rendP;
        private ParametricObjectRenderer _rendQ;

        private DummyCamera _cam = new DummyCamera();
        private DummyNode _node = new DummyNode();

        private BackgroundManager _backgroundManager;

        public ParametricSpaceExplorer()
        {
            InitializeComponent();

            _backgroundManager = new BackgroundManager(Color4.Black);

            _glContext = new DualGLContext(host, _backgroundManager);
            _glContext.Control.Paint += Control_PaintP;

            this.Closed += DisposeResources;
        }

        private void Control_PaintP(object sender, PaintEventArgs e) => Render();

        private void OnTick(object sender, EventArgs e)
        {
            Render();
        }

        private void DisposeResources(object sender, EventArgs e)
        {
            this.Closed -= DisposeResources;
            CompositionTarget.Rendering -= OnTick;

            _glContext.Control.Paint -= Control_PaintP;
            _rendP.Dispose();
            _rendQ.Dispose();
            _glContext.Dispose();
        }

        public void Show(INode pNode, INode qNode)
        {
            var pGen = (pNode.Renderer.GetGenerator() as ISurfaceGenerator);
            var qGen = (qNode.Renderer.GetGenerator() as ISurfaceGenerator);
            if (pGen == null || qGen == null) throw new InvalidOperationException("Provided nodes do not represent valid surfaces");
            Initialize(pGen.Surface, qGen.Surface);

            CompositionTarget.Rendering += OnTick;
            base.ShowDialog();
        }

        private void Initialize(ISurface p, ISurface q)
        {
            _rendP = new ParametricObjectRenderer(_glContext.ShaderProvider, new RawDataGenerator());
            _node.Assign(_rendP);
            SetupData(p, _rendP);

            _rendQ = new ParametricObjectRenderer(_glContext.ShaderProvider, new RawDataGenerator());
            _node.Assign(_rendQ);
            SetupData(q, _rendQ);
        }

        private void SetupData(ISurface surf, ParametricObjectRenderer rend)
        {
            var gen = rend.GetGenerator() as RawDataGenerator;
            if (gen == null) throw new InvalidOperationException("MeshGenerator is not a RawDataGenerator");

            //TODO: handle more curves on surfaces
            foreach(var c in surf.BoundingCurves)
            {
                var list = c.Curve.GetParameterList(surf);
                var uRng = surf.ParameterURange;
                var vRng = surf.ParameterVRange;
                gen.SetData(list.Select(x => new Vector3(
                    (x.X - uRng.X)/((uRng.Y - uRng.X)*0.5f)-1.0f, 
                    (x.Y - vRng.X)/((vRng.Y - vRng.X)*0.5f)-1.0f, 
                    0.0f)
                ).ToList());
            }
            gen.Size = RenderConstants.CURVE_SIZE;
            gen.SelectedColor = Color4.Red;
            gen.DefaultColor = Color4.Red;
            gen.DrawCallShaderType = DrawCallShaderType.Default;
        }

        private void Render()
        {
            _glContext.ScreenBufferManager.ResetScreenBuffer();
            Render(_glContext, _rendP, true);
            Render(_glContext, _rendQ, false);
            _glContext.Control.SwapBuffers();
        }

        private void Render(DualGLContext context, ParametricObjectRenderer rend, bool left)
        {
            context.MakeCurrent(left);
            context.ShaderProvider.UpdateShadersCameraMatrices(_cam);
            rend.Render(_cam, Matrix4.Identity, Matrix4.Identity);
        }
    }
}
