using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace avoCADo
{
    /// <summary>
    /// Handles control changes
    /// </summary>
    public partial class MainWindow
    {
        private void BindControls()
        {
            UpdateValues();
            torusXDivisions.ValueChanged += OnValueXChanged;
            torusYDivisions.ValueChanged += OnValueYChanged;
            torusMainRadius.ValueChanged += OnMainRadiusChanged;
            torusTubeRadius.ValueChanged += OnTubeRadiusChanged;
        }

        private void UnbindControls()
        {
            torusXDivisions.ValueChanged -= OnValueXChanged;
            torusYDivisions.ValueChanged -= OnValueYChanged;
            torusMainRadius.ValueChanged -= OnMainRadiusChanged;
            torusTubeRadius.ValueChanged -= OnTubeRadiusChanged;
        }

        private void UpdateValues()
        {
            torusXDivisions.Value = _torus.XDivisions;
            torusYDivisions.Value = _torus.YDivisions;
            torusMainRadius.Value = _torus.R;
            torusTubeRadius.Value = _torus.r;
        }

        private void OnValueXChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e)
        {
            _torus.SetXDivisions((int)e.NewValue);
        }

        private void OnValueYChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _torus.SetYDivisions((int)e.NewValue);
        }

        private void OnTubeRadiusChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _torus.SetTubeRadius((float)e.NewValue);
        }

        private void OnMainRadiusChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _torus.SetMainRadius((float)e.NewValue);
        }
    }
}
