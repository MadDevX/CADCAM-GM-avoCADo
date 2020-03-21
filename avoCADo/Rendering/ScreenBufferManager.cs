using System;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics.OpenGL;

public class ScreenBufferManager
{
    private Color _bgColor;

    public ScreenBufferManager(Color backgroundColor)
    {
        _bgColor = backgroundColor;
        GL.Enable(EnableCap.Lighting);
        GL.Enable(EnableCap.Light0);
        GL.Enable(EnableCap.Blend);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        GL.Enable(EnableCap.DepthTest);
    }

    public void ResetScreenBuffer()
    {
		GL.ClearColor(_bgColor);
		GL.Clear(ClearBufferMask.DepthBufferBit | ClearBufferMask.ColorBufferBit);
    }
}