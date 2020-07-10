using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace avoCADo
{
    public class DummyRenderer : IRenderer
    {
        public void Dispose() { }
        public IMeshGenerator GetGenerator() => null;
        public void Render(ICamera camera, Matrix4 localMatrix, Matrix4 parentMatrix) { }

        public void SetNode(INode node) { }
    }
}
