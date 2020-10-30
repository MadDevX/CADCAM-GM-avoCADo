using avoCADo.Shaders.ShaderWrappers;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo.Rendering.Renderers
{
    public class MeshRenderer : Renderer
    {
        public ITextureProvider TextureProvider { get; set; }
        public MillableSurfaceShaderWrapper Shader { get; }
        public MeshRenderer(MillableSurfaceShaderWrapper shader, Mesh mesh, ITextureProvider textureProvider) : base(shader, mesh)
        {
            Shader = shader;
            TextureProvider = textureProvider;
        }

        public override IMeshGenerator GetGenerator() => null;

        protected override void Draw(ICamera camera, Matrix4 localMatrix, Matrix4 parentMatrix)
        {
            if (TextureProvider != null)
            {
                TextureProvider.UpdateTexture();
                GL.ActiveTexture(TextureUnit.Texture0);
                GL.BindTexture(TextureTarget.Texture2D, TextureProvider.TextureHandle);
            }
            //GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
            GL.DrawElements(PrimitiveType.Triangles, _mesh.IndexCount, DrawElementsType.UnsignedInt, 0);
            //GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
        }

        protected override void SetBufferData()
        {
        }

        public void SetMesh(Mesh mesh)
        {
            if(_mesh != null)
            {
                _mesh.Dispose();
            }
            _mesh = mesh;
        }
    }
}
