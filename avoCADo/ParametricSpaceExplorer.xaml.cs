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
            var pGen = (pNode.Renderer.GetGenerator() as ISurfaceGenerator);
            var qGen = (qNode.Renderer.GetGenerator() as ISurfaceGenerator);
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
            SetupData(p, q, _rendPList);
            if (p == q)
            {
                SetupData(q, p, _rendQList, true);
            }
            else
            {
                SetupData(q, p, _rendQList);
            }
        }

        private void SetupData(ISurface surf, ISurface second, List<ParametricObjectRenderer> rendList, bool selfIntersectionQ = false)
        {
            //TODO: handle more curves on surfaces
            foreach(var c in surf.BoundingCurves)
            {
                if (second.BoundingCurves.Contains(c) == false) continue;
                var gen = new RawDataGenerator();
                var rend = new ParametricObjectRenderer(_glContext.ShaderProvider, gen);
                _node.Assign(rend);
                rendList.Add(rend);

                var list = c.Curve.GetParameterList(surf, selfIntersectionQ);
                var uRng = surf.ParameterURange;
                var vRng = surf.ParameterVRange;
                var positions = list.Select(x => new Vector3(
                    (x.X - uRng.X) / ((uRng.Y - uRng.X) * 0.5f) - 1.0f,
                    (x.Y - vRng.X) / ((vRng.Y - vRng.X) * 0.5f) - 1.0f,
                    0.0f)
                ).ToList();
                var indices = CorrectLooping(positions);
                gen.SetData(positions, indices);
                gen.Size = RenderConstants.CURVE_SIZE;
                gen.SelectedColor = Color4.Red;
                gen.DefaultColor = Color4.Red;
                gen.DrawCallShaderType = DrawCallShaderType.Default;
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

        #region Looping corrections

        private IList<uint> CorrectLooping(IList<Vector3> vects)
        {
            var alts = new Vector3[8];
            var alts2 = new Vector3[8];

            for (int i = vects.Count - 2; i >= 0; i--)
            {
                var pos = vects[i];
                var pos2 = vects[i + 1];
                var len = (pos - pos2).Length;
                FillAlts(alts, pos, true);
                FillAlts(alts2, pos2, false);

                var minDist = len;
                var minIdx = -1;
                for (int j = 0; j < 8; j++)
                {
                    var altDist = (pos - alts2[j]).Length;
                    if (altDist < minDist)
                    {
                        minIdx = j;
                        minDist = altDist;
                    }
                }

                //found better alternative
                if (minIdx != -1)
                {
                    vects.Insert(i + 1, alts2[minIdx]); //first half of "corrected" edge (normal Pos to corrected Pos2)
                    vects.Insert(i + 2, alts[minIdx]); //second half of "corrected" edge (corrected Pos to normal Pos2)
                }
            }

            var indices = GenerateLineIndices(vects.Count);
            RemoveOffNDCLines(vects, indices);
            return indices;
        }

        private List<uint> GenerateLineIndices(int vertexCount)
        {
            var len = (vertexCount - 1) * 2;
            var indices = new List<uint>(len);
            for (int i = 0; i < len; i++) indices.Add((uint)((i + 1) / 2));
            return indices;
        }

        private void RemoveOffNDCLines(IList<Vector3> vertices, List<uint> indices)
        {
            for (int i = indices.Count - 2; i >= 0; i -= 2)
            {
                var maxA = MaxAbsXYCoord(vertices[(int)indices[i]]);
                var maxB = MaxAbsXYCoord(vertices[(int)indices[i + 1]]);

                if (maxA > 1.0f && maxB > 1.0f)
                {
                    indices.RemoveAt(i + 1);
                    indices.RemoveAt(i);
                }
            }
        }

        private void FillAlts(Vector3[] altTable, Vector3 pos, bool negate)
        {
            var mult = negate ? -1.0f : 1.0f;
            altTable[0] = pos + mult * (2.0f * Vector3.UnitX);                        /* posXP   */ //XP - X/Y(axis) P/N(positive/negative) 
            altTable[1] = pos - mult * (2.0f * Vector3.UnitX);                        /* posXN   */
            altTable[2] = pos + mult * (2.0f * Vector3.UnitY);                        /* posYP   */
            altTable[3] = pos - mult * (2.0f * Vector3.UnitY);                        /* posYN   */
            altTable[4] = pos + mult * (2.0f * Vector3.UnitX + 2.0f * Vector3.UnitY); /* posXPYP */
            altTable[5] = pos + mult * (2.0f * Vector3.UnitX - 2.0f * Vector3.UnitY); /* posXPYN */
            altTable[6] = pos - mult * (2.0f * Vector3.UnitX + 2.0f * Vector3.UnitY); /* posXNYP */
            altTable[7] = pos - mult * (2.0f * Vector3.UnitX - 2.0f * Vector3.UnitY); /* posXNYN */
        }

        private float MaxAbsXYCoord(Vector3 pos)
        {
            return Math.Max(Math.Abs(pos.X), Math.Abs(pos.Y));
        }
        #endregion
    }
}
