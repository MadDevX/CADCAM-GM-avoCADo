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
        private GLControl _glControlP;
        private GLControl _glControlQ;
        private IShaderProvider _shaderProvider;

        private RawDataGenerator _genP;
        private ParametricObjectRenderer _rendP;
        private RawDataGenerator _genQ;
        private ParametricObjectRenderer _rendQ;

        private DummyCamera _cam = new DummyCamera();


        private BackgroundManager _backgroundManager;
        private ScreenBufferManager _screenBufferManager;

        public ParametricSpaceExplorer()
        {
            InitializeComponent();

            _glControlP = CreateGLControl(hostP);
            _glControlQ = CreateGLControl(hostQ);

            _backgroundManager = new BackgroundManager(Color4.White);
            _screenBufferManager = new ScreenBufferManager(_backgroundManager);
        }

        public void Initialize(IShaderProvider shaderProvider, ISurface p, ISurface q)
        {
            _shaderProvider = shaderProvider;

            _genP = new RawDataGenerator();
            _genQ = new RawDataGenerator();
            _rendP = new ParametricObjectRenderer(_shaderProvider, _genP);
            _rendQ = new ParametricObjectRenderer(_shaderProvider, _genQ);

            SetupData(p, _genP, _rendP, _glControlP);
            SetupData(q, _genQ, _rendQ, _glControlQ);

            Render();
        }

        private void SetupData(ISurface surf, RawDataGenerator gen, ParametricObjectRenderer rend, GLControl control)
        {
            foreach(var c in surf.BoundingCurves)
            {
                var list = c.Curve.GetParameterList(surf);
                gen.SetData(list.Select(x => new Vector3(x.X, x.Y, 0.0f)).ToList());
            }
            gen.Size = RenderConstants.CURVE_SIZE;
            gen.SelectedColor = Color4.Red;
            gen.DefaultColor = Color4.Red;
            gen.DrawCallShaderType = DrawCallShaderType.Default;
        }

        private void Render()
        {
            Render(_glControlP, _rendP);
            Render(_glControlQ, _rendQ);
        }

        private void Render(GLControl control, ParametricObjectRenderer rend)
        {
            control.MakeCurrent();
            _screenBufferManager.ResetScreenBuffer();
            rend.Render(_cam, Matrix4.Identity, Matrix4.Identity);
            GL.Finish();
            control.SwapBuffers();
        }

        private GLControl CreateGLControl(WindowsFormsHost host)
        {
            GLControl glControl = new GLControl(new OpenTK.Graphics.GraphicsMode(32, 24, 8, 8));
            glControl.Dock = DockStyle.Fill;
            host.Child = glControl;
            return glControl;
        }
    }
}
