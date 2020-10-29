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
        private ISelectionManager _selectionManager;

        public TorusGeneratorHandler(TorusGeneratorView torusView)
        {
            _torusView = torusView;
            _selectionManager = NodeSelection.Manager;
            Initialize();
        }

        private void Initialize()
        {
            _selectionManager.OnSelectionChanged += OnSelectionChanged;

            _torusView.xDivisions.ValueChanged += OnValueXChanged;
            _torusView.yDivisions.ValueChanged += OnValueYChanged;
            _torusView.mainRadius.ValueChanged += OnMainRadiusChanged;
            _torusView.tubeRadius.ValueChanged += OnTubeRadiusChanged;
            UpdateValues();
        }

        public void Dispose()
        {
            _selectionManager.OnSelectionChanged -= OnSelectionChanged;

            _torusView.xDivisions.ValueChanged -= OnValueXChanged;
            _torusView.yDivisions.ValueChanged -= OnValueYChanged;
            _torusView.mainRadius.ValueChanged -= OnMainRadiusChanged;
            _torusView.tubeRadius.ValueChanged -= OnTubeRadiusChanged;
        }

        private void OnSelectionChanged()
        {
            if (_selectionManager.MainSelection != null)
            {
                _torus = _selectionManager.MainSelection.GetComponent<Renderer>()?.GetGenerator() as TorusGenerator;
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
                _torusView.mainRadius.Value = ((TorusSurface)_torus.Surface).MainRadius;
                _torusView.tubeRadius.Value = ((TorusSurface)_torus.Surface).TubeRadius;
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
                _torus.XDivisions = (int)e.NewValue;
            }
        }

        private void OnValueYChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_torus != null)
            {
                _torus.YDivisions = (int)e.NewValue;
            }
        }

        private void OnTubeRadiusChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_torus != null)
            {
                ((TorusSurface)_torus.Surface).TubeRadius = (float)e.NewValue;
            }
        }

        private void OnMainRadiusChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_torus != null)
            {
                ((TorusSurface)_torus.Surface).MainRadius = (float)e.NewValue;
            }
        }
    }
}
