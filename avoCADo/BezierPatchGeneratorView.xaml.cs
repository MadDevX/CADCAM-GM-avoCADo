﻿using avoCADo.MeshGenerators;
using System;
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
    /// Interaction logic for BezierPatchGeneratorView.xaml
    /// </summary>
    public partial class BezierPatchGeneratorView : UserControl
    {
        private ISelectionManager _selectionManager;

        public BezierPatchGeneratorView()
        {
            InitializeComponent();
            _selectionManager = NodeSelection.Manager;

            Initialize();
        }

        private void Initialize()
        {
            Unloaded += OnUnload;
            _selectionManager.OnSelectionChanged += OnSelectionChanged;
        }

        private void OnUnload(object sender, RoutedEventArgs e)
        {
            _selectionManager.OnSelectionChanged -= OnSelectionChanged;
            Unloaded -= OnUnload;
        }

        private void OnSelectionChanged()
        {
            var gen = _selectionManager.MainSelection?.Renderer.GetGenerator() as BezierPatchGenerator;
            DataContext = gen;
            if (gen != null)
            {
                Visibility = Visibility.Visible;
                UpdateVectorsTextBlock();
            }
            else
            {
                var gen2 = _selectionManager.MainSelection?.Renderer.GetGenerator() as GregoryPatchGenerator;
                DataContext = gen2;
                if(gen2 != null)
                {
                    Visibility = Visibility.Visible;
                }
                else
                {
                    Visibility = Visibility.Collapsed;
                }
            }
        }

        private void UpdateVectorsTextBlock()
        {
            //if (_selectionManager.MainSelection?.Renderer.GetGenerator() is BezierPatchGenerator gen)
            //{
            //    var derivU = gen.Surface.DerivU(0.0f, 0.0f);
            //    var derivUU = gen.Surface.DerivUU(0.0f, 0.0f);
            //    var derivV = gen.Surface.DerivV(0.0f, 0.0f);
            //    var derivVV = gen.Surface.DerivVV(0.0f, 0.0f);
            //    var twist = gen.Surface.Twist(0.0f, 0.0f);
            //    var normal = gen.Surface.Normal(0.5f, 0.5f);
            //    vectorInfo.Text =
            //        $"Normal: {normal}\n" +
            //        $"DerivU: {derivU}\n" +
            //        $"DerivUU: {derivUU}\n" +
            //        $"DerivV: {derivV}\n" +
            //        $"DerivVV: {derivVV}\n" +
            //        $"Twist: {twist}";
            //}
        }
    }
}
