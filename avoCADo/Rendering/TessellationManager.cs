using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo
{
    public class TessellationManager
    {
        public TessellationManager()
        {
            Initialize();
        }

        private void Initialize()
        {
            GL.PatchParameter(PatchParameterInt.PatchVertices, 16);
        }
    }
}
