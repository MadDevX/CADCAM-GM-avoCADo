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

        public RenderLoop(GLControl glControl, ScreenBufferManager screenBufferManager, Scene scene, Camera camera)
        {
            _glControl = glControl;
            _screenBufferManager = screenBufferManager;
            _scene = scene;
            _camera = camera;
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

            for (int i = 0; i < _camera.Cycles; i++)
            {
                _camera.SetCycle(i);
                OnRenderLoop?.Invoke();
                _scene.Render(_camera);
            }
            //GL.Finish();

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
