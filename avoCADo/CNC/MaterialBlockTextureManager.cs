using avoCADo.Rendering.Renderers;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo.CNC
{
    public class MaterialBlockTextureManager : IDisposable
    {
        public int TextureHandle => _textureHandle;
        
        private int _textureHandle;

        public MaterialBlockTextureManager(int width, int height)
        {
            InitializeTexture(width, height);
        }

        private void InitializeTexture(int width, int height)
        {
            GL.CreateTextures(TextureTarget.Texture2D, 1, out _textureHandle);
            ResetTexture(width, height);
        }

        public void ResetTexture(int width, int height)
        {
            GL.ActiveTexture(TextureUnit.Texture0); //TODO: check, maybe use different texture unit
            GL.BindTexture(TextureTarget.Texture2D, TextureHandle);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.R32f, width, height, 0, PixelFormat.Red, PixelType.Float, IntPtr.Zero); //TODO: verify
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        public void UpdateTexture(int width, int height, float[] heightMap)
        {
            GL.BindTexture(TextureTarget.Texture2D, TextureHandle);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.R32f, width, height, 0, PixelFormat.Red, PixelType.Float, heightMap);
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        public void Dispose()
        {
            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.DeleteTextures(1, ref _textureHandle);
        }
    }
}
