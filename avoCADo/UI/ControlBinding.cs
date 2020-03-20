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
            DataChanged += UpdateValues;
            
        }

        private void UnbindControls()
        {
            torusXDivisions.ValueChanged -= OnValueXChanged;
            torusYDivisions.ValueChanged -= OnValueYChanged;
            torusMainRadius.ValueChanged -= OnMainRadiusChanged;
            torusTubeRadius.ValueChanged -= OnTubeRadiusChanged;
            DataChanged -= UpdateValues;
        }

        private void UpdateValues()
        {
            var torus = Torus as TorusGenerator;
            if (torus != null)
            {
                if (generatorView.Visibility != Visibility.Visible) generatorView.Visibility = Visibility.Visible;
                torusXDivisions.Value = torus.XDivisions;
                torusYDivisions.Value = torus.YDivisions;
                torusMainRadius.Value = torus.R;
                torusTubeRadius.Value = torus.r;
            }
            else
            {
                generatorView.Visibility = Visibility.Collapsed;
            }
        }

        private void OnValueXChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e)
        {
            var torus = Torus as TorusGenerator;
            if (torus != null)
                torus.SetXDivisions((int)e.NewValue);
        }

        private void OnValueYChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var torus = Torus as TorusGenerator;
            if (torus != null)
                torus.SetYDivisions((int)e.NewValue);
        }

        private void OnTubeRadiusChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var torus = Torus as TorusGenerator;
            if (torus != null)
                torus.SetTubeRadius((float)e.NewValue);
        }

        private void OnMainRadiusChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var torus = Torus as TorusGenerator;
            if (torus != null)
                torus.SetMainRadius((float)e.NewValue);
        }
    }
}
