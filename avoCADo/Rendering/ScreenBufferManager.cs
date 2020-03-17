using System;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics.OpenGL;

/// <summary>
/// The renderer.
/// </summary>
public class ScreenBufferManager
{
    private Color _bgColor;

    public ScreenBufferManager(Color backgroundColor)
    {
        _bgColor = backgroundColor;
    }

    public void ResetScreenBuffer()
    {
        GL.Enable( EnableCap.Lighting );
		GL.Enable( EnableCap.Light0 );
		GL.Enable( EnableCap.Blend );
		GL.BlendFunc(BlendingFactor.SrcAlpha , BlendingFactor.OneMinusSrcAlpha);
		GL.Enable( EnableCap.DepthTest );

		GL.ClearColor( _bgColor/*Color.FromArgb( 200 , Color.LightBlue )*/ );
		GL.Clear(ClearBufferMask.DepthBufferBit | ClearBufferMask.ColorBufferBit);
    }
}