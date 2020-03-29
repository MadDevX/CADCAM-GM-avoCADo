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

    public class CurveRenderer : MeshRenderer
    {
        private Shader _curveShader;
        private int _curveShaderModelMatrixLocation;
        private int _curveShaderColorLocation;

        public CurveRenderer(Shader curveShader, Shader shader, IMeshGenerator meshGenerator) : base(shader, meshGenerator)
        {
            _curveShader = curveShader;
            _curveShaderModelMatrixLocation = GL.GetUniformLocation(_curveShader.Handle, "model");
            _curveShaderColorLocation = GL.GetUniformLocation(_curveShader.Handle, "color");
        }

        protected override void Draw()
        {
        }
    }
}
