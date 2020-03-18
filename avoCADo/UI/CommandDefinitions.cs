using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace avoCADo
{
    public static class CommandDefinitions
    {
        public static readonly RoutedUICommand Torus = new RoutedUICommand
            (
                "New Torus",
                "Torus",
                typeof(CommandDefinitions),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.T, ModifierKeys.Control)
                }
            );

        public static readonly RoutedUICommand Point = new RoutedUICommand
            (
                "New Point",
                "Point",
                typeof(CommandDefinitions),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.P, ModifierKeys.Control)
                }
            );
    }
}
