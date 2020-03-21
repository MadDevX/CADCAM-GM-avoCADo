using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace avoCADo
{
    public class TransformHandler : IDisposable
    {
        private TransformView _transformView;
        private IUpdateLoop _updateLoop;
        private float _refreshDelay;
        private float _timer;

        public TransformHandler(TransformView transformView, IUpdateLoop updateLoop, float refreshDelay = 0.1f)
        {
            _transformView = transformView;
            _updateLoop = updateLoop;
            _refreshDelay = refreshDelay;
            Initialize();
        }

        private void Initialize()
        {
            NodeSelection.Manager.OnSelectionChanged += OnSelectionChanged;
            _updateLoop.OnUpdateLoop += OnUpdate;
        }

        public void Dispose()
        {
            NodeSelection.Manager.OnSelectionChanged -= OnSelectionChanged;
            _updateLoop.OnUpdateLoop -= OnUpdate;
        }

        private void OnSelectionChanged()
        {
            if (NodeSelection.Manager.MainSelection != null)
            {
                if (_transformView.Visibility != Visibility.Visible) _transformView.Visibility = Visibility.Visible;
                _transformView.Transform = NodeSelection.Manager.MainSelection.Transform;
            }
            else
            {
                _transformView.Visibility = Visibility.Collapsed;
            }
        }

        private void OnUpdate(float deltaTime)
        {
            _timer += deltaTime;
            if(_timer >= _refreshDelay)
            {
                if (_transformView.Transform != null)
                {
                    _transformView.UpdateValues();
                }
                _timer = 0.0f;
            }
        }
    }
}
