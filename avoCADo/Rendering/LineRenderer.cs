using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo
{
    public class LineRenderer : MeshRenderer
    {
        public LineRenderer(Shader shader, IMeshGenerator meshGenerator) : base(shader, meshGenerator)
        {
        }

        protected override void Draw()
        {
            GL.DrawElements(PrimitiveType.Lines, _indexCount, DrawElementsType.UnsignedInt, 0);
        }
    }
}
