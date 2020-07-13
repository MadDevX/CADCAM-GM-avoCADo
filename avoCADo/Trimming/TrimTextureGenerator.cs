using AForge.Imaging.Filters;
using OpenTK.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo
{
    public static class TrimTextureGenerator
    {
        public static Bitmap FillBitmap(Bitmap toFill, int x, int y)
        {
            var flood = new PointedColorFloodFill(Color.FromArgb(0, 0, 0));
            flood.Tolerance = Color.FromArgb(0, 0, 0);
            flood.StartingPoint = new AForge.IntPoint(x, y);

            return flood.Apply(toFill);
        }
    }
}
