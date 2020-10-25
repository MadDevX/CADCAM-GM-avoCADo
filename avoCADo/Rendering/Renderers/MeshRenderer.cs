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
        private ITextureProvider _textureProvider;
        public MeshRenderer(ShaderWrapper shader, Mesh mesh, ITextureProvider textureProvider) : base(shader, mesh)
        {
            _textureProvider = textureProvider;
        }

        public override IMeshGenerator GetGenerator() => null;

        protected override void Draw(ICamera camera, Matrix4 localMatrix, Matrix4 parentMatrix)
        {
            if (_textureProvider != null)
            {
                GL.ActiveTexture(TextureUnit.Texture0);
                GL.BindTexture(TextureTarget.Texture2D, _textureProvider.TextureHandle);
            }
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
            GL.DrawElements(PrimitiveType.Triangles, _mesh.IndexCount, DrawElementsType.UnsignedInt, 0);
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
        }

        protected override void SetBufferData()
        {
        }
    }
}
