using OpenTK;
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
        private ParametricObjectRenderer _renderer;

        public ParametricSpaceExplorer()
        {
            InitializeComponent();

            _glControlP = CreateGLControl(hostP);
            _glControlQ = CreateGLControl(hostQ);
        }

        public void Initialize(ShaderProvider shaderProvider)
        {
        }

        public void Render(ISurface p, ISurface q)
        {
            foreach(var c in p.BoundingCurves)
            {
                var list = c.GetParameterList(p);
            }
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
