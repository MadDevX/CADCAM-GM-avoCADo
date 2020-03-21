using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace avoCADo
{
    public class TorusGeneratorHandler : IDisposable
    {
        private TorusGeneratorView _torusView;
        private TorusGenerator _torus;


        public TorusGeneratorHandler(TorusGeneratorView torusView)
        {
            _torusView = torusView;
            Initialize();
        }

        private void Initialize()
        {
            NodeSelection.Manager.OnSelectionChanged += OnSelectionChanged;

            _torusView.xDivisions.ValueChanged += OnValueXChanged;
            _torusView.yDivisions.ValueChanged += OnValueYChanged;
            _torusView.mainRadius.ValueChanged += OnMainRadiusChanged;
            _torusView.tubeRadius.ValueChanged += OnTubeRadiusChanged;
            UpdateValues();
        }

        public void Dispose()
        {
            NodeSelection.Manager.OnSelectionChanged -= OnSelectionChanged;

            _torusView.xDivisions.ValueChanged -= OnValueXChanged;
            _torusView.yDivisions.ValueChanged -= OnValueYChanged;
            _torusView.mainRadius.ValueChanged -= OnMainRadiusChanged;
            _torusView.tubeRadius.ValueChanged -= OnTubeRadiusChanged;
        }

        private void OnSelectionChanged()
        {
            if (NodeSelection.Manager.MainSelection != null)
            {
                _torus = NodeSelection.Manager.MainSelection.Renderer.GetGenerator() as TorusGenerator;
            }
            else
            {
                _torus = null;
            }
            UpdateValues();
        }

        private void UpdateValues()
        {
            if (_torus != null)
            {
                if (_torusView.Visibility != Visibility.Visible) _torusView.Visibility = Visibility.Visible;
                _torusView.xDivisions.Value = _torus.XDivisions;
                _torusView.yDivisions.Value = _torus.YDivisions;
                _torusView.mainRadius.Value = _torus.R;
                _torusView.tubeRadius.Value = _torus.r;
            }
            else
            {
                _torusView.Visibility = Visibility.Collapsed;
            }
        }

        private void OnValueXChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e)
        {
            if (_torus != null)
            {
                _torus.SetXDivisions((int)e.NewValue);
            }
        }

        private void OnValueYChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_torus != null)
            {
                _torus.SetYDivisions((int)e.NewValue);
            }
        }

        private void OnTubeRadiusChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_torus != null)
            {
                _torus.SetTubeRadius((float)e.NewValue);
            }
        }

        private void OnMainRadiusChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_torus != null)
            {
                _torus.SetMainRadius((float)e.NewValue);
            }
        }
    }
}
