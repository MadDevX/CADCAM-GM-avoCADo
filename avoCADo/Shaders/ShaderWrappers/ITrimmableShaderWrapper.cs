using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo
{
    public interface ITrimmableShaderWrapper
    {
        void SetTrimTexture(int textureUnit);

        void SetPatchCoords(Vector2 coords);

        void SetPatchDimensions(Vector2 dims);

        void SetFlipUV(bool flipUV);
        void SetTrim(bool trim);
        void SetFlipTrim(bool flipTrim);
    }
}
