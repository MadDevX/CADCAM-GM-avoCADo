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

        private static int TEXTURE_SIZE = 4096;
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

            ResetTexture();
        }

        private void ResetTexture()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, FramebufferHandle);
            GL.Viewport(0, 0, TEXTURE_SIZE, TEXTURE_SIZE);
            GL.ClearColor(Color4.White);
            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        private void InitializeTexture()
        {
            GL.CreateFramebuffers(1, out _framebufferHandle);
            GL.CreateTextures(TextureTarget.Texture2D, 1, out _textureHandle);

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, TextureHandle);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, TEXTURE_SIZE, TEXTURE_SIZE, 0, PixelFormat.Rgb, PixelType.UnsignedByte, IntPtr.Zero);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, FramebufferHandle);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, TextureHandle, 0);
            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

            GL.BindTexture(TextureTarget.Texture2D, TextureHandle);
        }

        /// <summary>
        /// ISurface q represents "complimentary" surface to the one we try to trim (only common intersections between p & q are trimmed).
        /// </summary>
        /// <param name="q"></param>
        /// <param name="isP"></param>
        public void UpdateTrimTexture(ISurface q, bool isP)
        {
            GL.ActiveTexture(TextureUnit.Texture0);
            if (RenderToTexture(q))
            {
                UpdateOutlineBitmap(isP);
                UpdateTextureData(isP, Surface.ULoop, Surface.VLoop);
            }
        }

        private bool RenderToTexture(ISurface q)
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, FramebufferHandle);
            GL.Viewport(0, 0, TEXTURE_SIZE, TEXTURE_SIZE);
            GL.ClearColor(Color4.White);
            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.Disable(EnableCap.DepthTest);
            var rends = GetRenderers(q, Registry.ShaderProvider);
            Registry.ShaderProvider.UpdateShadersCameraMatrices(_cam);
            GL.LineWidth(1.0f);
            foreach(var rend in rends)
            {
                rend.Render(_cam, Matrix4.Identity, Matrix4.Identity);
                //rend.Dispose();
            }
            GL.Enable(EnableCap.DepthTest);

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

            return rends.Count > 0;
        }

        private List<ParametricObjectRenderer> GetRenderers(ISurface q, ShaderProvider shaderProvider)
        {
            var list = new List<ParametricObjectRenderer>();
            //TODO: handle selfintersections
            ParametricSpaceConverter.SetupData(Surface, q, list, shaderProvider, Color4.Black);
            return list;
        }

        private void UpdateTextureData(bool isP, bool uLoop, bool vLoop)
        {
            var result = TrimTextureGenerator.FillBitmap(_bitmap, 0, 0, uLoop, vLoop);
            result.Save($"filledTexture{(isP?0:1)}.bmp");
            var bits = result.LockBits(_rect, ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            GL.BindTexture(TextureTarget.Texture2D, TextureHandle);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, TEXTURE_SIZE, TEXTURE_SIZE, 0, PixelFormat.Rgb, PixelType.UnsignedByte, bits.Scan0);
            GL.BindTexture(TextureTarget.Texture2D, 0);
            result.UnlockBits(bits);
            result.Dispose();
        }

        private void UpdateOutlineBitmap(bool isP)
        {
            BitmapData bits = _bitmap.LockBits(_rect, ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            //GL.BindTexture(TextureTarget.Texture2D, TextureHandle);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, FramebufferHandle);
            GL.ReadPixels(0, 0, TEXTURE_SIZE, TEXTURE_SIZE, PixelFormat.Rgb, PixelType.UnsignedByte, bits.Scan0);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            //GL.BindTexture(TextureTarget.Texture2D, 0);
            _bitmap.UnlockBits(bits);
            _bitmap.Save($"renderedTexture{(isP?0:1)}.bmp");
        }

        public void Dispose()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.DeleteFramebuffers(1, ref _framebufferHandle);
            GL.DeleteTextures(1, ref _textureHandle);
        }
    }
}
