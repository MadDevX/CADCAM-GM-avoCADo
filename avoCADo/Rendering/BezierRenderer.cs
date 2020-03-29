using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace avoCADo
{
    public class BezierRenderer : IRenderer
    {
        private LineRenderer _edgeRenderer;
        private LineRenderer _curveRenderer;

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public IMeshGenerator GetGenerator()
        {
            throw new NotImplementedException();
        }

        public void Render(Camera camera, Matrix4 localMatrix, Matrix4 parentMatrix)
        {
            _edgeRenderer.Render(camera, localMatrix, parentMatrix);
            _curveRenderer.Render(camera, localMatrix, parentMatrix);
        }
    }
}
