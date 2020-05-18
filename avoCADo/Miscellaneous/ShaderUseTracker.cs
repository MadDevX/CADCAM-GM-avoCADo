using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo.Miscellaneous
{
    public static class ShaderUseTracker
    {
        private static int _currentHandle = -69;

        public static bool IsBound(int handle)
        {
            return _currentHandle == handle;
        }

        public static void BindShader(int handle)
        {
            if(_currentHandle != handle)
            {
                GL.UseProgram(handle);
                _currentHandle = handle;
            }
        }
    }
}
