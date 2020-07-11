using System;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace avoCADo
{
    public class ScreenBufferManager
    {
        private ViewportManager _viewportManager;
        private BackgroundManager _backgroundManager;

        public ScreenBufferManager(ViewportManager viewportManager, BackgroundManager backgroundManager, GLControl control)
        {
            _viewportManager = viewportManager;
            _backgroundManager = backgroundManager;
            SetupContext(control);
        }

        private void SetupContext(GLControl control)
        {
            control.MakeCurrent();
            GL.Enable(EnableCap.Lighting);
            GL.Enable(EnableCap.Light0);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GL.Enable(EnableCap.DepthTest);
        }

        public void ResetScreenBuffer(bool viewportOnly = false)
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            if(viewportOnly)
            {
                GL.Enable(EnableCap.ScissorTest);
                var lu = _viewportManager.ViewportLeftUpper;
                var sz = _viewportManager.ViewportSize;
                GL.Scissor(lu.X, lu.Y, sz.Width, sz.Height);
            }
            GL.ClearColor(_backgroundManager.BackgroundColor);
            GL.Clear(ClearBufferMask.DepthBufferBit | ClearBufferMask.ColorBufferBit);
            if(viewportOnly)
            {
                GL.Disable(EnableCap.ScissorTest);
            }
        }
    }
}