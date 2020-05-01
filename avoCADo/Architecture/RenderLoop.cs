using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo
{
    public class RenderLoop : IDisposable, IRenderLoop
    {
        public event Action OnRenderLoop;

        private GLControl _glControl;
        private ScreenBufferManager _screenBufferManager;
        private Scene _scene;
        private Camera _camera;
        private QuadOverlayRenderer _quadOverlayRenderer;
        private FramebufferManager _framebufferManager;


        public RenderLoop(GLControl glControl, ScreenBufferManager screenBufferManager, Scene scene, Camera camera, FramebufferManager framebufferManager, QuadOverlayRenderer quadRenderer)
        {
            _glControl = glControl;
            _screenBufferManager = screenBufferManager;
            _scene = scene;
            _camera = camera;
            _quadOverlayRenderer = quadRenderer;
            _framebufferManager = framebufferManager;

            Initialize();
        }

        private void Initialize()
        {
            _glControl.Paint += GLControlOnPaint;
        }

        public void Dispose()
        {
            _glControl.Paint -= GLControlOnPaint;
        }

        private void GLControlOnPaint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            #region Timer
            //_stopwatch.Start();
            #endregion

            _glControl.MakeCurrent();
            _screenBufferManager.ResetScreenBuffer();
            _framebufferManager.ClearFrameBuffers(_camera.Cycles);
            _framebufferManager.SetTextureUnits();

            for (int i = 0; i < _camera.Cycles; i++)
            {
                //GL.Clear(ClearBufferMask.DepthBufferBit); //-- used for multirendering straight to screen (causes depth buffer issues)
                _camera.SetCycle(i);
                _framebufferManager.SetFramebuffer(i);

                OnRenderLoop?.Invoke();
                _scene.Render(_camera);
            }

            _quadOverlayRenderer.Render();

            _glControl.SwapBuffers();

            #region Timer
            //_frames++;
            //if (_stopwatch.ElapsedTicks >= Stopwatch.Frequency)
            //{
            //    this.Title = $"{_frames} FPS";
            //    _frames = 0;
            //    _stopwatch.Reset();
            //}
            #endregion
        }
    }
}
