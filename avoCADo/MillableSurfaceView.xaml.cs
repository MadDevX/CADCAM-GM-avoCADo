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
            if(_millable != null)
            {
                _millable.OnSimulationFinished -= OnSimFinished;
            }
            _millable = _selectionManager.MainSelection?.GetComponent<MillableSurface>();
            if(_millable != null)
            {
                Visibility = Visibility.Visible;
                _millable.OnSimulationFinished += OnSimFinished;
                SetStartButtonText();
            }
            else
            {
                Visibility = Visibility.Collapsed;
            }
            DataContext = _millable;
        }

        private void SetStartButtonText()
        {
            btnSkipSimulation.IsEnabled = _millable.SimulationInProgress;
            btnLoadFiles.IsEnabled = !_millable.SimulationInProgress;
            if (_millable.SimulationInProgress)
            {
                btnStartSimulation.Content = _millable.Paused ? "Resume Simulation" : "Pause Simulation";
            }
            else
            {
                btnStartSimulation.Content = "Start Simulation";
            }
        }

        private void OnSimFinished()
        {
            SetStartButtonText();
        }

        private List<string> _names = new List<string>();
        private void btnLoadFiles_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Multiselect = true;
                ofd.ShowDialog();
                var filesSelected = ofd.FileNames.Length > 0;
                _names.Clear();
                if (filesSelected)
                {
                    var instSetList = new List<CNCInstructionSet>();
                    foreach (var filename in ofd.FileNames)
                    {
                        var instSet = CNCInstructionParser.ParsePathFile(filename);
                        instSetList.Add(instSet);
                        _names.Add(NameGenerator.DiscardPath(filename, discardExtension: false));
                    }
                    _millable.SetPaths(instSetList);
                }
                UpdateLoadedFilesText();
                SetStartButtonText();
                btnStartSimulation.IsEnabled = filesSelected;

            }
            catch (Exception eOfd)
            {
                System.Windows.MessageBox.Show(eOfd.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateLoadedFilesText()
        {
            StringBuilder newText = new StringBuilder();
            newText.Append("Loaded Files: ");
            foreach(var name in _names)
            {
                newText.Append($"{name}; ");
            }
            tbLoadedfiles.Text = newText.ToString();
        }

        private void btnStartSimulation_Click(object sender, RoutedEventArgs e)
        {
            if (_millable.SimulationInProgress)
            {
                _millable.Paused = !_millable.Paused;
            }
            else
            {
                _millable.StartSimulation();
            }
            SetStartButtonText();
        }

        private void btnSkipSimulation_Click(object sender, RoutedEventArgs e)
        {
            _millable.SkipSimulation();
        }

        private void btnResetMaterial_Click(object sender, RoutedEventArgs e)
        {
            _millable.ResetMaterial();
        }
    }
}
