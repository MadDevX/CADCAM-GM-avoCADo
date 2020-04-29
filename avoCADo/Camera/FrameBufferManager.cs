using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
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

        public FramebufferManager(int bufferCount, ViewportManager viewportManager)
        {
            _viewportManager = viewportManager;

            _bufferCount = bufferCount;
            _FBOs = new int[_bufferCount];
            _textures = new int[_bufferCount];
            _RBOs = new int[_bufferCount];
            InitializeFramebuffers();
            InitializeTextureUnits();
        }

        private void InitializeTextureUnits()
        {
            for (int i = 0; i < _bufferCount; i++)
            {
                GL.ActiveTexture(TextureUnit.Texture0 + i);
                GL.BindTexture(TextureTarget.Texture2D, _textures[i]);
            }
        }

        private void InitializeFramebuffers()
        {
            GL.CreateFramebuffers(_bufferCount, _FBOs);
            GL.CreateTextures(TextureTarget.Texture2D, _bufferCount, _textures);
            GL.CreateRenderbuffers(_bufferCount, _RBOs);

            UpdateTextures();

            var status = GL.CheckNamedFramebufferStatus(_FBOs[0], FramebufferTarget.Framebuffer);
            if(status != FramebufferStatus.FramebufferComplete)
            {
                throw new InvalidOperationException();
            }
        }

        private void UpdateTextures()
        {
                //GL.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter)
            for (int i = 0; i < _bufferCount; i++)
            {
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, _FBOs[i]);
                
                //initialize texture
                GL.BindTexture(TextureTarget.Texture2D, _textures[i]);
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, _viewportManager.Width, _viewportManager.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);
                GL.BindTexture(TextureTarget.Texture2D, 0);
                //bind texture
                GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, _textures[i], 0);
                
                //initialize renderbuffer
                GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, _RBOs[i]);
                GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.Depth24Stencil8, _viewportManager.Width, _viewportManager.Height);
                GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);
                //bind renderbuffer
                GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthStencilAttachment, RenderbufferTarget.Renderbuffer, _RBOs[i]);

                GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            }
        }

        public void Dispose()
        {
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

        public void ClearFrameBuffers()
        {
            for(int i = 0; i < _FBOs.Length; i++)
            {
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, _FBOs[i]);
                GL.ClearColor(0.0f, 0.0f, 0.0f, 0.0f);
                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
                //GL.ClearNamedFramebuffer(_FBOs[i], ClearBufferCombined.DepthStencil, )
            }
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }
    }
}
