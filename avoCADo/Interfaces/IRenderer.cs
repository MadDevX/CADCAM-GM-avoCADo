using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo
{
    public interface IRenderer : IDisposable
    {
        void Render(ICamera camera, Matrix4 localMatrix, Matrix4 parentMatrix);
        IMeshGenerator GetGenerator();
    }
}
