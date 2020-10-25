using OpenTK;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo.CNC
{
    public class Algorithms
    {
        public static void Bresenham(int x0, int y0, int x1, int y1, List<Point> points)
        {
            int dx, dy, p, x, y;

            dx = x1 - x0;
            dy = y1 - y0;

            x = x0;
            y = y0;

            p = 2 * dy - dx;

            while (x < x1)
            {
                if (p >= 0)
                {
                    points.Add(new Point(x, y));
                    y = y + 1;
                    p = p + 2 * dy - 2 * dx;
                }
                else
                {
                    points.Add(new Point(x, y));
                    p = p + 2 * dy;
                }
                x = x + 1;
            }
        }
    }
}
