using avoCADo.CNC;
using avoCADo.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace avoCADo
{
    /// <summary>
    /// Interaction logic for MillableSurfaceView.xaml
    /// </summary>
    public partial class MillableSurfaceView : System.Windows.Controls.UserControl
    {
        private ISelectionManager _selectionManager;
        private MillableSurface _millable;

        public MillableSurfaceView()
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
            _millable = _selectionManager.MainSelection?.GetComponent<MillableSurface>();
            if(_millable != null)
            {
                Visibility = Visibility.Visible;
            }
            else
            {
                Visibility = Visibility.Collapsed;
            }
            DataContext = _millable;
        }

        private void btnLoadFiles_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Multiselect = true;
                ofd.ShowDialog();
                if (ofd.FileNames.Length > 0)
                {
                    var instSetList = new List<CNCInstructionSet>();
                    foreach (var filename in ofd.FileNames)
                    {
                        instSetList.Add(CNCInstructionParser.ParsePathFile(filename));
                    }
                    _millable.SetPaths(instSetList);
                }

            }
            catch (Exception eOfd)
            {
                System.Windows.MessageBox.Show(eOfd.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnStartSimulation_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnSkipSimulation_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnResetMaterial_Click(object sender, RoutedEventArgs e)
        {
            _millable.ResetMaterial();
        }
    }
}
