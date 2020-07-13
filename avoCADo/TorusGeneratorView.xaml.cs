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
    /// Interaction logic for TorusGeneratorView.xaml
    /// </summary>
    public partial class TorusGeneratorView : UserControl
    {
        private ISelectionManager _selectionManager;
        public TorusGeneratorView()
        {
            InitializeComponent();
            _selectionManager = NodeSelection.Manager;
            _selectionManager.OnSelectionChanged += OnSelectionChanged;
            Unloaded += OnUnload;
        }

        private void OnUnload(object sender, RoutedEventArgs e)
        {
            Unloaded -= OnUnload;
            _selectionManager.OnSelectionChanged -= OnSelectionChanged;
        }

        private void OnSelectionChanged()
        {
            if (_selectionManager.MainSelection != null)
            {
                var gen = _selectionManager.MainSelection.Renderer.GetGenerator() as TorusGenerator;
                DataContext = gen;
            }
            else
            {
                DataContext = null;
            }
        }
    }
}
