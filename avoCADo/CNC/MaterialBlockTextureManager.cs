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
    public class MaterialBlockTextureManager : IDisposable, ITextureProvider
    {
        public int TextureHandle => _textureHandle;
        
        private int _textureHandle;
        private readonly MaterialBlock _block;

        public MaterialBlockTextureManager(MaterialBlock block)
        {
            _block = block;
            InitializeTexture();
        }

        private void InitializeTexture()
        {
            GL.CreateTextures(TextureTarget.Texture2D, 1, out _textureHandle);

            GL.ActiveTexture(TextureUnit.Texture0); //TODO: check, maybe use different texture unit
            GL.BindTexture(TextureTarget.Texture2D, TextureHandle);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.R32f, _block.Width, _block.Height, 0, PixelFormat.Red, PixelType.Float, IntPtr.Zero); //TODO: verify
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        public void UpdateTexture()
        {
            GL.BindTexture(TextureTarget.Texture2D, TextureHandle);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.R32f, _block.Width, _block.Height, 0, PixelFormat.Red, PixelType.Float, _block.HeightMap);
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        public void Dispose()
        {
            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.DeleteTextures(1, ref _textureHandle);
        }
    }
}
