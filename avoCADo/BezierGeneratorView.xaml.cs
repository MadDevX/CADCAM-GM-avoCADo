﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace avoCADo
{
    /// <summary>
    /// Interaction logic for BezierGeneratorView.xaml
    /// </summary>
    public partial class BezierGeneratorView : UserControl
    {
        public BezierGeneratorView()
        {
            InitializeComponent();
            Unloaded += OnUnload;
            NodeSelection.Manager.OnSelectionChanged += OnSelectionChanged;
        }

        private void OnUnload(object sender, RoutedEventArgs e)
        {
            NodeSelection.Manager.OnSelectionChanged -= OnSelectionChanged;
            Unloaded -= OnUnload;
        }

        private void OnSelectionChanged()
        {
            DataContext = NodeSelection.Manager.MainSelection?.Renderer.GetGenerator() as BezierGenerator;
            if (DataContext == null)
            {
                Visibility = Visibility.Collapsed;
            }
            else
            {
                Visibility = Visibility.Visible;
            }
        }
    }
}