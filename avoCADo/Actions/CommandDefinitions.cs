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

        public static readonly RoutedUICommand Bezier = new RoutedUICommand
            (
                "New Bezier Curve",
                "Bezier",
                typeof(CommandDefinitions),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.B, ModifierKeys.Control)
                }
            );

        public static readonly RoutedUICommand BSpline = new RoutedUICommand
            (
                "New BSpline Curve",
                "BSpline",
                typeof(CommandDefinitions),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.S, ModifierKeys.Control)
                }
            );

        public static readonly RoutedUICommand InterpolatingC2 = new RoutedUICommand
            (
                "New Interpolating C2 Curve",
                "InterpolatingC2",
                typeof(CommandDefinitions),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.I, ModifierKeys.Control)
                }
            );

        public static readonly RoutedUICommand BezierPatchC0 = new RoutedUICommand
            (
                "New Bezier C0 Patch",
                "BezierPatchC0",
                typeof(CommandDefinitions),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.B, ModifierKeys.Control | ModifierKeys.Shift)
                }
            );

        public static readonly RoutedUICommand BezierPatchC2 = new RoutedUICommand
            (
                "New Bezier C2 Patch",
                "BezierPatchC2",
                typeof(CommandDefinitions),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.S, ModifierKeys.Control | ModifierKeys.Shift)
                }
            );

        public static readonly RoutedUICommand MergePoints = new RoutedUICommand
            (
                "Merge Points",
                "MergePoints",
                typeof(CommandDefinitions),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.M, ModifierKeys.Control)
                }
            );

        public static readonly RoutedUICommand FillHole = new RoutedUICommand
            (
                "Fill Hole",
                "FillHole",
                typeof(CommandDefinitions),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.F, ModifierKeys.Control)
                }
            );

        public static readonly RoutedUICommand FindIntersection = new RoutedUICommand
            (
                "Find Intersection",
                "FindIntersection",
                typeof(CommandDefinitions),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.C, ModifierKeys.Control)
                }
            );

        public static readonly RoutedUICommand ShowParametricExplorer = new RoutedUICommand
            (
                "Show Parametric Explorer",
                "ShowParametricExplorer",
                typeof(CommandDefinitions)
            );

        public static readonly RoutedUICommand DeleteNode = new RoutedUICommand
            (
                "Delete",
                "Delete",
                typeof(CommandDefinitions)
            );

        public static readonly RoutedUICommand TryDeleteSelected = new RoutedUICommand
            (
                "Delete Selected",
                "TryDeleteSelected",
                typeof(CommandDefinitions),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.Delete)
                }
            );

        public static readonly RoutedUICommand LoadScene = new RoutedUICommand
            (
                "Load Scene",
                "LoadScene",
                typeof(CommandDefinitions),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.F9)
                }
            );

        public static readonly RoutedUICommand SaveScene = new RoutedUICommand
            (
                "Save Scene",
                "SaveScene",
                typeof(CommandDefinitions),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.F5)
                }
            );
        public static readonly RoutedUICommand NewScene = new RoutedUICommand
            (
                "New Scene",
                "NewScene",
                typeof(CommandDefinitions),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.N, ModifierKeys.Control | ModifierKeys.Shift)
                }
            );
    }
}
