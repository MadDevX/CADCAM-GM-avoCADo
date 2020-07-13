using avoCADo.Architecture;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo.Trimming
{
    using PixelFormat = OpenTK.Graphics.OpenGL.PixelFormat;
    public class TrimTextureProvider : IDisposable
    {
        private DummyCamera _cam = new DummyCamera();

        private static int TEXTURE_SIZE = 1024;
        private static Bitmap _bitmap = new Bitmap(TEXTURE_SIZE, TEXTURE_SIZE, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
        private static Rectangle _rect = new Rectangle(0, 0, TEXTURE_SIZE, TEXTURE_SIZE);
        public ISurface Surface { get; }

        private int _framebufferHandle;
        private int _textureHandle;
        public int TextureHandle => _textureHandle;
        public int FramebufferHandle => _framebufferHandle;

        public TrimTextureProvider(ISurface surface)
        {
            Surface = surface;

            InitializeTexture();

            var status = GL.CheckNamedFramebufferStatus(FramebufferHandle, FramebufferTarget.Framebuffer);
            if (status != FramebufferStatus.FramebufferComplete)
            {
                throw new InvalidOperationException();
            }
        }

        private void InitializeTexture()
        {
            GL.CreateFramebuffers(1, out _framebufferHandle);
            GL.CreateTextures(TextureTarget.Texture2D, 1, out _textureHandle);

            GL.BindTexture(TextureTarget.Texture2D, TextureHandle);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, TEXTURE_SIZE, TEXTURE_SIZE, 0, PixelFormat.Rgb, PixelType.UnsignedByte, IntPtr.Zero);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            GL.BindTexture(TextureTarget.Texture2D, 0);

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, FramebufferHandle);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, TextureHandle, 0);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

            GL.BindTexture(TextureTarget.Texture2D, TextureHandle);
        }

        public void UpdateTrimTexture(ISurface q)
        {
            RenderToTexture(q);
            GL.ActiveTexture(TextureUnit.Texture0);
            UpdateOutlineBitmap();
            UpdateTextureData();
        }

        private void RenderToTexture(ISurface q)
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, FramebufferHandle);
            GL.Viewport(0, 0, TEXTURE_SIZE, TEXTURE_SIZE);
            GL.ClearColor(Color4.White);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            var rends = GetRenderers(q, Registry.ShaderProvider);
            Registry.ShaderProvider.UpdateShadersCameraMatrices(_cam);
            foreach(var rend in rends)
            {
                rend.Render(_cam, Matrix4.Identity, Matrix4.Identity);
            }

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        private List<ParametricObjectRenderer> GetRenderers(ISurface q, ShaderProvider shaderProvider)
        {
            var list = new List<ParametricObjectRenderer>();
            //TODO: handle selfintersections
            ParametricSpaceConverter.SetupData(Surface, q, list, shaderProvider);
            return list;
        }

        private void UpdateTextureData()
        {
            var result = TrimTextureGenerator.FillBitmap(_bitmap, 0, 0); //TODO: find coords
            var bits = result.LockBits(_rect, ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            GL.BindTexture(TextureTarget.Texture2D, TextureHandle);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, TEXTURE_SIZE, TEXTURE_SIZE, 0, PixelFormat.Rgb, PixelType.UnsignedByte, bits.Scan0);
            GL.BindTexture(TextureTarget.Texture2D, 0);
            result.UnlockBits(bits);
            result.Dispose();
        }

        private void UpdateOutlineBitmap()
        {
            BitmapData bits = _bitmap.LockBits(_rect, ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            GL.BindTexture(TextureTarget.Texture2D, TextureHandle);
            GL.ReadPixels(0, 0, TEXTURE_SIZE, TEXTURE_SIZE, PixelFormat.Rgb, PixelType.UnsignedByte, bits.Scan0);
            GL.BindTexture(TextureTarget.Texture2D, 0);
            _bitmap.UnlockBits(bits);
        }

        public void Dispose()
        {
            GL.DeleteTextures(1, ref _textureHandle);
            GL.DeleteBuffers(1, ref _framebufferHandle);
        }
    }
}
