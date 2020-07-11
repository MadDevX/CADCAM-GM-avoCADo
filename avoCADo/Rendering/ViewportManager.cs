using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo
{
    public class ViewportManager : IDisposable
    {
        public event Action<Size> OnViewportChanged;

        private GLControl _control;

        public float AspectRatio => (float)_control.Width / _control.Height;
        public int Width => _control.Width;
        public int Height => _control.Height;

        public Point ViewportLeftUpper { get; private set; }
        public Size ViewportSize { get; private set; }
        public ViewportManager(GLControl control)
        {
            _control = control;
            Init();
        }

        public void Init()
        {
            _control.SizeChanged += SetViewport;
            _control.MakeCurrent();
            GL.Viewport(_control.Size);
        }

        public void Dispose()
        {
            _control.SizeChanged -= SetViewport;
        }

        private void SetViewport(object sender, EventArgs e)
        {
            _control.MakeCurrent();
            ViewportLeftUpper = Point.Empty;
            ViewportSize = _control.Size;
            GL.Viewport(_control.Size);
            OnViewportChanged?.Invoke(_control.Size);
        }

        public void SetCustomViewport(Point leftUpperCorner, Size size)
        {
            _control.MakeCurrent();
            ViewportLeftUpper = leftUpperCorner;
            ViewportSize = size;
            GL.Viewport(leftUpperCorner, size);
            OnViewportChanged?.Invoke(size);
        }
    }
}
