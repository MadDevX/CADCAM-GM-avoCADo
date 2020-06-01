using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace avoCADo
{
    public static class IconProvider
    {
        public static ImageSource iconPoint = (BitmapImage)Application.Current.Resources["iconPoint"];
        public static ImageSource iconCurve = (BitmapImage)Application.Current.Resources["iconCurve"];
        public static ImageSource iconSurface = (BitmapImage)Application.Current.Resources["iconSurface"];
        public static ImageSource iconScene = (BitmapImage)Application.Current.Resources["iconScene"];
        public static ImageSource iconTorus = (BitmapImage)Application.Current.Resources["iconTorus"];

        public static ImageSource GetIcon(ObjectType type)
        {
            switch(type)
            {
                case ObjectType.Point:
                    return iconPoint;
                case ObjectType.Torus:
                    return iconTorus;
                case ObjectType.BezierCurveC0:
                case ObjectType.BezierCurveC2:
                case ObjectType.InterpolatingCurve:
                    return iconCurve;
                case ObjectType.BezierPatchC0:
                case ObjectType.BezierPatchC2:
                    return iconSurface;
                case ObjectType.Scene:
                    return iconScene;
                default:
                    return iconScene;
            }
        }
    }
}
