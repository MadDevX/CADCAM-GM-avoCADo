using System;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace avoCADo
{
    public class ScreenBufferManager
    {
        private BackgroundManager _backgroundManager;

        public ScreenBufferManager(BackgroundManager backgroundManager, GLControl control)
        {
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

        public void ResetScreenBuffer()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.ClearColor(_backgroundManager.BackgroundColor);
            GL.Clear(ClearBufferMask.DepthBufferBit | ClearBufferMask.ColorBufferBit);
        }
    }
}