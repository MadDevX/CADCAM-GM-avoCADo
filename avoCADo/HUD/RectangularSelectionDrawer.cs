using OpenTK;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo
{
    public class RectangularSelectionDrawer : IDisposable
    {
        private readonly IRenderLoop _renderLoop;
        private readonly ScreenSelectionHandler _screenSelectionHandler;

        private readonly RectangleSelectionRenderer _renderer;

        public RectangularSelectionDrawer(IRenderLoop renderLoop, ScreenSelectionHandler screenSelectionHandler, SimpleShaderWrapper shaderWrapper, GLControl control)
        {
            _renderLoop = renderLoop;
            _screenSelectionHandler = screenSelectionHandler;
            _renderer = new RectangleSelectionRenderer(shaderWrapper, _screenSelectionHandler, control);
            Initialize();
        }

        private void Initialize()
        {
            _renderLoop.OnRenderLoop += RenderSelection;
        }

        public void Dispose()
        {
            _renderLoop.OnRenderLoop -= RenderSelection;
            _renderer.Dispose();
        }

        private void RenderSelection()
        {
            if (_screenSelectionHandler.State == ScreenSelectionHandler.HandlingState.InProgress && 
                _screenSelectionHandler.CurrentSelectionDistance > _screenSelectionHandler.SingleSelectionThreshold)
            {
                _renderer.Render();
            }
        }
    }
}
