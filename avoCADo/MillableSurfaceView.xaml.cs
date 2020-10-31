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
            cbToolType.ItemsSource = Enum.GetValues(typeof(CNCToolType)).Cast<CNCToolType>();
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
                _millable.OnCNCSimulatorUpdated -= UpdateToolInfo;
                _millable.UpdateProgress -= UpdateProgress;
            }
            _millable = _selectionManager.MainSelection?.GetComponent<MillableSurface>();
            if(_millable != null)
            {
                Visibility = Visibility.Visible;
                _millable.OnSimulationFinished += OnSimFinished;
                _millable.OnCNCSimulatorUpdated += UpdateToolInfo;
                _millable.UpdateProgress += UpdateProgress;
                SetStartButtonText();
                UpdateToolInfo();
                UpdateLoadedFilesText();
            }
            else
            {
                Visibility = Visibility.Collapsed;
            }
            DataContext = _millable;
        }

        private void UpdateProgress()
        {
            progressBar.Value = _millable.SimulationProgress;
        }

        private void UpdateToolInfo()
        {
            var instSet = _millable?.Simulator?.InstructionSet;
            if (instSet != null)
            {
                var tool = instSet.Tool;
                tblkToolInfo.Text = $"Tool | Type: {tool.Type}; Radius: {tool.Radius * 1000.0f}mm; Height: {tool.Height * 100.0f}cm";
            }
            else
            {
                tblkToolInfo.Text = "Tool | No tool info available";
            }
        }

        private void SetStartButtonText()
        {

            btnStartSimulation.IsEnabled = _millable.InstructionSetCount > 0 && !_millable.IsSkipping;
            btnSkipSimulation.IsEnabled = _millable.SimulationInProgress && !_millable.IsSkipping;
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
            UpdateToolInfo();
        }

        private void btnLoadFiles_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Multiselect = true;
                ofd.ShowDialog();
                var filesSelected = ofd.FileNames.Length > 0;
                if (filesSelected)
                {
                    var instSetList = new List<CNCInstructionSet>();
                    foreach (var filename in ofd.FileNames)
                    {
                        var instSet = CNCInstructionParser.ParsePathFile(filename, _millable.ToolHeight);
                        instSetList.Add(instSet);
                    }
                    _millable.SetPaths(instSetList);
                }
                UpdateLoadedFilesText();
                SetStartButtonText();

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
            foreach(var name in _millable.InstructionSetNames)
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
            btnSkipSimulation.IsEnabled = !_millable.IsSkipping;
        }

        private void btnResetMaterial_Click(object sender, RoutedEventArgs e)
        {
            _millable.ResetMaterial();
        }
    }
}
