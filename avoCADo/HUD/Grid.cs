using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo
{
    public class Grid : IDisposable
    {

        private Camera _camera;
        private IRenderLoop _renderLoop;

        private IRenderer _gridRenderer;

        public bool Enabled { get; set; } = true;

        public Grid(Camera camera, IRenderLoop renderLoop, IRenderer gridRenderer)
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
                _gridRenderer.Render(_camera, Matrix4.Identity, Matrix4.Identity); //TODO : modulo translate along xz plane
            }
        }
    }
}
