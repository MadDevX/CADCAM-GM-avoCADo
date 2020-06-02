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
    /// Interaction logic for BezierGeneratorView.xaml
    /// </summary>
    public partial class BezierGeneratorView : UserControl
    {
        private ISelectionManager _selectionManager;

        public BezierGeneratorView()
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
            var gen =_selectionManager.MainSelection?.Renderer.GetGenerator() as BezierGenerator;
            if (gen != null)
            {
                if (gen.Curve is IVirtualControlPoints)
                {
                    cbBernstein.Visibility = Visibility.Visible;
                }
                else
                {
                    cbBernstein.Visibility = Visibility.Collapsed;
                }
                DataContext = gen;
                if (DataContext == null)
                {
                    Visibility = Visibility.Collapsed;
                }
                else
                {
                    Visibility = Visibility.Visible;
                }
            }
            else
            {
                var gen2 = NodeSelection.Manager.MainSelection?.Renderer.GetGenerator() as BezierGeneratorGeometry;
                if (gen2 != null)
                {
                    if (gen2.Curve is IVirtualControlPoints)
                    {
                        cbBernstein.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        cbBernstein.Visibility = Visibility.Collapsed;
                    }
                    DataContext = gen2;
                    if (DataContext == null)
                    {
                        Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        Visibility = Visibility.Visible;
                    }
                }
                else
                {
                    Visibility = Visibility.Collapsed;
                }
            }
        }
    }
}
