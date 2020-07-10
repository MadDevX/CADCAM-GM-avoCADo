using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo
{
    public class Grid : IDisposable
    {

        private ICamera _camera;
        private IRenderLoop _renderLoop;

        private IRenderer _gridRenderer;

        public bool Enabled { get; set; } = true;

        public Grid(ICamera camera, IRenderLoop renderLoop, IRenderer gridRenderer)
        {
            _camera = camera;
            _renderLoop = renderLoop;
            _gridRenderer = gridRenderer;
            Initialize();
        }

        private void Initialize()
        {
            _renderLoop.OnRenderLoop += OnRender;
        }

        public void Dispose()
        {
            _renderLoop.OnRenderLoop -= OnRender;
        }

        private void OnRender()
        {
            if (Enabled)
            {
                var pos = _camera.Position;
                GL.DepthMask(false);
                _gridRenderer.Render(_camera, Matrix4.CreateTranslation(new Vector3((int)pos.X, 0.0f, (int)pos.Z)), Matrix4.Identity);
                GL.DepthMask(true);
            }
        }
    }
}
