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

        public static ImageSource GetIcon(NodeType type)
        {
            switch(type)
            {
                case NodeType.Point:
                    return iconPoint;
                case NodeType.Curve:
                    return iconCurve;
                case NodeType.Surface:
                    return iconSurface;
                case NodeType.Scene:
                    return iconScene;
                default:
                    return iconScene;
            }
        }
    }
}
