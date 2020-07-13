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
        private bool _paused = false;
        public bool Paused 
        {
            get => _paused;
            set
            {
                _paused = value;
                if (value == true) _glControl.Context.MakeCurrent(null);
            }
        }

        private GLControl _glControl;
        private ScreenBufferManager _screenBufferManager;
        private readonly ViewportManager _viewportManager;
        private SceneManager _sceneManager;
        private Camera _camera;
        private QuadOverlayRenderer _quadOverlayRenderer;
        private FramebufferManager _framebufferManager;
        private ShaderProvider _shaderProvider;

        public RenderLoop(GLControl glControl, ScreenBufferManager screenBufferManager, ViewportManager viewportManager, SceneManager sceneManager, Camera camera, FramebufferManager framebufferManager, QuadOverlayRenderer quadRenderer, ShaderProvider shaderProvider)
        {
            _glControl = glControl;
            _screenBufferManager = screenBufferManager;
            _viewportManager = viewportManager;
            _sceneManager = sceneManager;
            _camera = camera;
            _quadOverlayRenderer = quadRenderer;
            _framebufferManager = framebufferManager;
            _shaderProvider = shaderProvider;

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
            if (Paused) return;
            #region Timer
            //_stopwatch.Start();
            #endregion

            _glControl.MakeCurrent();
            _viewportManager.ResetViewport();
            _screenBufferManager.ResetScreenBuffer();
            _framebufferManager.ClearFrameBuffers(_camera.Cycles);
            _framebufferManager.SetTextureUnits();

            GL.Enable(EnableCap.AlphaTest);
            GL.AlphaFunc(AlphaFunction.Greater, 0.0f);

            for (int i = 0; i < _camera.Cycles; i++)
            {
                //GL.Clear(ClearBufferMask.DepthBufferBit); //-- used for multirendering straight to screen (causes depth buffer issues)
                _camera.SetCycle(i);
                _framebufferManager.SetFramebuffer(i);
                _shaderProvider.UpdateShadersCameraMatrices(_camera);
                OnRenderLoop?.Invoke();
                _sceneManager.CurrentScene.Render(_camera);
            }

            _quadOverlayRenderer.Render();

            GL.Disable(EnableCap.AlphaTest);
            GL.Finish();
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
