using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace avoCADo
{
    public static class UtilityCommandDefinitions
    {
        public static readonly RoutedUICommand LocalMode = new RoutedUICommand
            (
                "Local Transformation Mode",
                "LocalMode",
                typeof(CommandDefinitions),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.F1)
                }
            );
        public static readonly RoutedUICommand CursorMode = new RoutedUICommand
            (
                "Cursor Transformation Mode",
                "CursorMode",
                typeof(CommandDefinitions),
                new InputGestureCollection()
                {
                            new KeyGesture(Key.F2)
                }
            );
        public static readonly RoutedUICommand SnapNone = new RoutedUICommand
            (
                "No snapping",
                "SnapNone",
                typeof(CommandDefinitions),
                new InputGestureCollection()
                {
                            new KeyGesture(Key.NumPad1)
                }
            );
        public static readonly RoutedUICommand SnapValue = new RoutedUICommand
            (
                "Snap value",
                "SnapNone",
                typeof(CommandDefinitions),
                new InputGestureCollection()
                {
                            new KeyGesture(Key.NumPad2)
                }
            );
        public static readonly RoutedUICommand SnapToGrid = new RoutedUICommand
            (
                "Snap to grid",
                "SnapNone",
                typeof(CommandDefinitions),
                new InputGestureCollection()
                {
                            new KeyGesture(Key.NumPad3)
                }
            );
        public static readonly RoutedUICommand SnapGrid01 = new RoutedUICommand
            (
                "0.1",
                "SnapGrid01",
                typeof(CommandDefinitions),
                new InputGestureCollection()
                {
                            new KeyGesture(Key.NumPad7)
                }
            );
        public static readonly RoutedUICommand SnapGrid025 = new RoutedUICommand
            (
                "0.25",
                "SnapGrid1",
                typeof(CommandDefinitions),
                new InputGestureCollection()
                {
                            new KeyGesture(Key.NumPad8)
                }
            );
        public static readonly RoutedUICommand SnapGrid05 = new RoutedUICommand
            (
                "0.5",
                "SnapGrid001",
                typeof(CommandDefinitions),
                new InputGestureCollection()
                {
                            new KeyGesture(Key.NumPad9)
                }
            );
    }
}
