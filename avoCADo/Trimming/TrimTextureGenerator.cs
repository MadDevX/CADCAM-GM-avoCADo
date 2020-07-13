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
        private static Color BLANK_COLOR = Color.FromArgb(255, 255, 255, 255);

        public static Bitmap FillBitmap(Bitmap toFill, int x, int y, bool uLoop, bool vLoop)
        {
            var flood = new PointedColorFloodFill(Color.FromArgb(0, 0, 0));
            flood.Tolerance = Color.FromArgb(0, 0, 0);

            var col = toFill.GetPixel(x, y);
            if (toFill.GetPixel(x, y) != BLANK_COLOR)
            {
                x = (x + 1) % toFill.Width;
                while(toFill.GetPixel(x, y) != BLANK_COLOR)
                {
                    y = (y + 1) % toFill.Height;
                    if (y == 0) x = (x + 1) % toFill.Width;

                    if (y == 0 && x == 0) throw new InvalidOperationException("Provided texture does not contain blank-colored pixels to be filled");
                }
            }

            flood.StartingPoint = new AForge.IntPoint(x, y);

            var filled =  flood.Apply(toFill);
            if(x == 0 && y == 0)
            {
                if (uLoop)
                {
                    Bitmap filled2;
                    if (filled.GetPixel(toFill.Width - 1, 0) == BLANK_COLOR)
                    {
                        flood.StartingPoint = new AForge.IntPoint(toFill.Width - 1, 0); //top right corner
                        filled2 = flood.Apply(filled); //top right corner apply
                        filled.Dispose();
                        filled = filled2;
                    }
                    if (vLoop)
                    {
                        if (filled.GetPixel(0, toFill.Height - 1) == BLANK_COLOR)
                        {
                            flood.StartingPoint = new AForge.IntPoint(0, toFill.Height - 1); //lower left corner
                            filled2 = flood.Apply(filled); //lower left corner apply
                            filled.Dispose();
                            filled = filled2;
                        }
                        if (filled.GetPixel(toFill.Width - 1, toFill.Height - 1) == BLANK_COLOR)
                        {
                            flood.StartingPoint = new AForge.IntPoint(toFill.Width - 1, toFill.Height - 1); //lower right corner
                            filled2 = flood.Apply(filled); //lower right corner apply
                            filled.Dispose();
                            filled = filled2;
                        }
                    }
                    return filled;
                }
                if(vLoop)
                {
                    if (filled.GetPixel(0, toFill.Height - 1) == BLANK_COLOR)
                    {
                        flood.StartingPoint = new AForge.IntPoint(0, toFill.Height - 1); //lower left corner
                        var filled2 = flood.Apply(filled); //lower left corner apply
                        filled.Dispose();
                        filled = filled2;
                    }
                }
            }
            return filled;
            //TODO: handle looping also along x/y edges (also, virtually impossible case)
            //if(x == 0 && y == to) TODO: process all corners (it's virtually impossible though to encounter these)
            //if(x == toFill.Width - 1 )

        }
    }
}
