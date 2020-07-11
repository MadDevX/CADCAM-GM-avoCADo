using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo.ParametricObjects
{
    public static class ParameterHelper
    {
        /// <summary>
        /// Useful for looped parametrizations (moves UV back to parametrization bounds)
        /// </summary>
        /// <param name="surf"></param>
        /// <param name="uv"></param>
        /// <returns></returns>
        public static Vector2 CorrectUV(ISurface surf, Vector2 uv)
        {
            if(surf.ULoop)
            {
                var rng = surf.ParameterURange;
                var len = rng.Y - rng.X;
                uv.X = (((uv.X - rng.X) % len) + len) % len + rng.X;
            }
            if(surf.VLoop)
            {
                var rng = surf.ParameterVRange;
                var len = rng.Y - rng.X;
                uv.Y = (((uv.Y - rng.X) % len) + len) % len + rng.X;
            }
            return uv;
        }

        public static Vector4 CorrectUVST(ISurface p, ISurface q, Vector4 uvst)
        {
            var uv = new Vector2(uvst.X, uvst.Y);
            var st = new Vector2(uvst.Z, uvst.W);

            uv = CorrectUV(p, uv);
            st = CorrectUV(q, st);

            return new Vector4(uv.X, uv.Y, st.X, st.Y);
        }
    }
}
