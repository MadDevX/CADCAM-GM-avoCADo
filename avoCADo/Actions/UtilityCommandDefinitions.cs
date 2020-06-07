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
        public static readonly RoutedUICommand Undo = new RoutedUICommand
            (
                "Undo",
                "Undo",
                typeof(CommandDefinitions),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.Z, ModifierKeys.Control)
                }
            );
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
                    new KeyGesture(Key.NumPad1, ModifierKeys.Control)
                }
            );
        public static readonly RoutedUICommand SnapValue = new RoutedUICommand
            (
                "Snap value",
                "SnapNone",
                typeof(CommandDefinitions),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.NumPad2, ModifierKeys.Control)
                }
            );
        public static readonly RoutedUICommand SnapToGrid = new RoutedUICommand
            (
                "Snap to grid",
                "SnapNone",
                typeof(CommandDefinitions),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.NumPad3, ModifierKeys.Control)
                }
            );
        public static readonly RoutedUICommand SnapGrid01 = new RoutedUICommand
            (
                "0.1",
                "SnapGrid01",
                typeof(CommandDefinitions),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.NumPad7, ModifierKeys.Control)
                }
            );
        public static readonly RoutedUICommand SnapGrid025 = new RoutedUICommand
            (
                "0.25",
                "SnapGrid1",
                typeof(CommandDefinitions),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.NumPad8, ModifierKeys.Control)
                }
            );
        public static readonly RoutedUICommand SnapGrid05 = new RoutedUICommand
            (
                "0.5",
                "SnapGrid001",
                typeof(CommandDefinitions),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.NumPad9, ModifierKeys.Control)
                }
            );
        public static readonly RoutedUICommand EnableViewPlaneTranslate = new RoutedUICommand
            (
                "Enable View Plane Translate",
                "ViewPlaneTranslate",
                typeof(CommandDefinitions)
            );
    }
}
