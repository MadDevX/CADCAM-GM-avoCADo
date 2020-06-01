using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace avoCADo
{
    public partial class MainWindow
    {
        private void LocalCursorMode_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void LocalMode_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            _transformationsManager.Mode = HUD.TransformationMode.Local;
            menuItemLocalMode.IsChecked = true;
            menuItemCursorMode.IsChecked = false;
        }

        private void CursorMode_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            _transformationsManager.Mode = HUD.TransformationMode.Cursor;
            menuItemCursorMode.IsChecked = true;
            menuItemLocalMode.IsChecked = false;
        }

        private void SnapMode_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void SnapNone_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            _transformationsManager.SnapMode = HUD.SnapMode.None;
            menuItemSnapNone.IsChecked = true;
            menuItemSnapValue.IsChecked = false;
            menuItemSnapToGrid.IsChecked = false;
        }
        private void SnapValue_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            _transformationsManager.SnapMode = HUD.SnapMode.SnapValue;
            menuItemSnapNone.IsChecked = false;
            menuItemSnapValue.IsChecked = true;
            menuItemSnapToGrid.IsChecked = false;
        }
        private void SnapToGrid_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            _transformationsManager.SnapMode = HUD.SnapMode.SnapToGrid;
            menuItemSnapNone.IsChecked = false;
            menuItemSnapValue.IsChecked = false;
            menuItemSnapToGrid.IsChecked = true;
        }

        private void SnapGrid_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void SnapGrid01_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            _transformationsManager.SnapValue = 0.1f;
            menuItemSnapGrid01.IsChecked = true;
            menuItemSnapGrid025.IsChecked = false;
            menuItemSnapGrid05.IsChecked = false;
        }
        private void SnapGrid025_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            _transformationsManager.SnapValue = 0.25f;
            menuItemSnapGrid01.IsChecked = false;
            menuItemSnapGrid025.IsChecked = true;
            menuItemSnapGrid05.IsChecked = false;
        }
        private void SnapGrid05_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            _transformationsManager.SnapValue = 0.5f;
            menuItemSnapGrid01.IsChecked = false;
            menuItemSnapGrid025.IsChecked = false;
            menuItemSnapGrid05.IsChecked = true;
        }
    }
}
