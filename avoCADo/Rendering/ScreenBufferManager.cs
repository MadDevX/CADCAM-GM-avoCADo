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

        public ScreenBufferManager(BackgroundManager backgroundManager)
        {
            _backgroundManager = backgroundManager;
            GL.Enable(EnableCap.Lighting);
            GL.Enable(EnableCap.Light0);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            //GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.DstAlpha);// - nope, render to texture, then merge and fill screen
            GL.Enable(EnableCap.DepthTest);
            //GL.Disable(EnableCap.DepthTest);
        }

        public void ResetScreenBuffer()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.ClearColor(_backgroundManager.BackgroundColor);
            GL.Clear(ClearBufferMask.DepthBufferBit | ClearBufferMask.ColorBufferBit);
        }
    }
}