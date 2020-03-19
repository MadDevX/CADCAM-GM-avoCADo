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
        Matrix4 GetLocalModelMatrix(Transform transform);
        void Render(Transform transform, Camera camera, Matrix4 parentMatrix);
    }
}
