using System;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

public class ScreenBufferManager
{
    private Color4 _bgColor;

    public ScreenBufferManager(Color4 backgroundColor)
    {
        _bgColor = backgroundColor;
        GL.Enable(EnableCap.Lighting);
        GL.Enable(EnableCap.Light0);
        GL.Enable(EnableCap.Blend);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        //GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.DstAlpha); - nope, render to texture, then merge and fill screen
        GL.Enable(EnableCap.DepthTest);
    }

    public void ResetScreenBuffer()
    {
		GL.ClearColor(_bgColor);
		GL.Clear(ClearBufferMask.DepthBufferBit | ClearBufferMask.ColorBufferBit);
    }
}