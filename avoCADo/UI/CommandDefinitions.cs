﻿using System;
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

        public static readonly RoutedUICommand DeleteNode = new RoutedUICommand
            (
                "Delete",
                "Delete",
                typeof(CommandDefinitions)
            );
    }
}
