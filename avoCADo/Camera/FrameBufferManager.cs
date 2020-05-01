using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo
{
    public class FramebufferManager : IDisposable
    {
        private ViewportManager _viewportManager;

        private int[] _FBOs;
        private int[] _textures;
        private int[] _RBOs;
        private int _bufferCount;

        private BackgroundManager _backgroundManager;

        private int _samples = 8;

        public FramebufferManager(int bufferCount, ViewportManager viewportManager, BackgroundManager backgroundManager)
        {
            _viewportManager = viewportManager;
            _backgroundManager = backgroundManager;

            _bufferCount = bufferCount;
            _FBOs = new int[_bufferCount];
            _textures = new int[_bufferCount];
            _RBOs = new int[_bufferCount];
            InitializeFramebuffers();
            SetTextureUnits();

            _viewportManager.OnViewportChanged += UpdateTextureSize;
        }

        private void InitializeFramebuffers()
        {
            GL.Enable(EnableCap.Multisample);
            GL.CreateFramebuffers(_bufferCount, _FBOs);
            GL.CreateTextures(TextureTarget.Texture2DMultisample, _bufferCount, _textures);
            GL.CreateRenderbuffers(_bufferCount, _RBOs);
            InitializeTextures();

            CheckFramebuffers();
        }

        private void CheckFramebuffers()
        {
            for (int i = 0; i < _bufferCount; i++)
            {
                var status = GL.CheckNamedFramebufferStatus(_FBOs[i], FramebufferTarget.Framebuffer);
                if (status != FramebufferStatus.FramebufferComplete)
                {
                    throw new InvalidOperationException();
                }
            }
        }

        public void SetTextureUnits()
        {
            for (int i = 0; i < _bufferCount; i++)
            {
                GL.ActiveTexture(TextureUnit.Texture0 + i);
                GL.BindTexture(TextureTarget.Texture2DMultisample, _textures[i]);
                GL.Enable(EnableCap.Texture2D);
            }
        }

        private void InitializeTextures()
        {
                //GL.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter)
            for (int i = 0; i < _bufferCount; i++)
            {
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, _FBOs[i]);
                
                //initialize texture
                GL.BindTexture(TextureTarget.Texture2DMultisample, _textures[i]);
                GL.TexImage2DMultisample(TextureTargetMultisample.Texture2DMultisample, _samples, PixelInternalFormat.Rgba, _viewportManager.Width, _viewportManager.Height, true);
                //GL.TexParameter(TextureTarget.Texture2DMultisample, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
                //GL.TexParameter(TextureTarget.Texture2DMultisample, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
                
                GL.BindTexture(TextureTarget.Texture2DMultisample, 0);
                //bind texture
                GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2DMultisample, _textures[i], 0);
                
                //initialize renderbuffer
                GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, _RBOs[i]);
                GL.RenderbufferStorageMultisample(RenderbufferTarget.Renderbuffer, _samples, RenderbufferStorage.Depth24Stencil8, _viewportManager.Width, _viewportManager.Height);
                GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);
                //bind renderbuffer
                GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthStencilAttachment, RenderbufferTarget.Renderbuffer, _RBOs[i]);

                GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            }
        }


        private void UpdateTextureSize(Size size)
        {
            for(int i = 0; i < _bufferCount; i++)
            {
                GL.BindTexture(TextureTarget.Texture2DMultisample, _textures[i]);
                GL.TexImage2DMultisample(TextureTargetMultisample.Texture2DMultisample, _samples, PixelInternalFormat.Rgba, size.Width, size.Height, true);
                GL.BindTexture(TextureTarget.Texture2DMultisample, 0);

                GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, _RBOs[i]);
                GL.RenderbufferStorageMultisample(RenderbufferTarget.Renderbuffer, _samples, RenderbufferStorage.Depth24Stencil8, size.Width, size.Height);
                GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);
            }
        }


        public void Dispose()
        {
            _viewportManager.OnViewportChanged -= UpdateTextureSize;
            GL.DeleteRenderbuffers(_bufferCount, _RBOs);
            GL.DeleteTextures(_bufferCount, _textures);
            GL.DeleteBuffers(_bufferCount, _FBOs);
        }

        public void SetFramebuffer(int index)
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, _FBOs[index]);
            //GL.NamedFramebufferDrawBuffer(0, DrawBufferMode.None);
            //GL.NamedFramebufferDrawBuffer(_FBOs[index], DrawBufferMode.Back);
            //GL.NamedFramebufferDrawBuffer(0, DrawBufferMode.Back);
        }

        public void ClearFrameBuffers(int cycles)
        {
            if (cycles > _FBOs.Length) throw new ArgumentException("more cycles than allocated FBOs");
            for(int i = 0; i < _FBOs.Length; i++)
            {
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, _FBOs[i]);
                if (i >= cycles) GL.ClearColor(new Color4(0.0f, 0.0f, 0.0f, 0.0f));
                else GL.ClearColor(CorrectedBackgroundColor(cycles));
                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
                //GL.ClearNamedFramebuffer(_FBOs[i], ClearBufferCombined.DepthStencil, )
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            }
        }

        private Color4 CorrectedBackgroundColor(int cycles)
        {
            var frac = 1.0f / (float)cycles;
            var color = _backgroundManager.BackgroundColor;
            color.R *= frac;
            color.G *= frac;
            color.B *= frac;
            color.A = 1.0f;
            return color;
        }
    }
}
