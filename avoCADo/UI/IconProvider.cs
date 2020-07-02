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
        private static ImageSource iconPoint = (BitmapImage)Application.Current.Resources["iconPoint"];
        private static ImageSource iconCurve = (BitmapImage)Application.Current.Resources["iconCurve"];
        private static ImageSource iconSurface = (BitmapImage)Application.Current.Resources["iconSurface"];
        private static ImageSource iconScene = (BitmapImage)Application.Current.Resources["iconScene"];
        private static ImageSource iconTorus = (BitmapImage)Application.Current.Resources["iconTorus"];

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
                case ObjectType.IntersectionCurve:
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
