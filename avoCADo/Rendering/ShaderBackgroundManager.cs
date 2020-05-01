using OpenTK.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo
{
    public class ShaderBackgroundManager : IDisposable
    {
        private BackgroundManager _backgroundManager;
        private List<ShaderWrapper> _shaderWrappers = new List<ShaderWrapper>();

        public ShaderBackgroundManager(BackgroundManager backgroundManager, params ShaderWrapper[] shaderWrappers)
        {
            _backgroundManager = backgroundManager;

            foreach(var wrapper in shaderWrappers)
            {
                _shaderWrappers.Add(wrapper);
            }

            Initialize();
        }

        private void Initialize()
        {
            _backgroundManager.OnBackgroundColorChanged += OnBackgroundColorChanged;
            OnBackgroundColorChanged();
        }

        public void Dispose()
        { 
            _backgroundManager.OnBackgroundColorChanged -= OnBackgroundColorChanged;
        }

        public void AddWrapper(ShaderWrapper wrapper)
        {
            _shaderWrappers.Add(wrapper);
            wrapper.SetBackgroundColor(_backgroundManager.BackgroundColor);
        }

        public void RemoveWrapper(ShaderWrapper wrapper)
        {
            _shaderWrappers.Remove(wrapper);
        }

        private void OnBackgroundColorChanged()
        {
            UpdateBackgroundColor(_backgroundManager.BackgroundColor);
        }

        private void UpdateBackgroundColor(Color4 color)
        {
            foreach(var wrapper in _shaderWrappers)
            {
                wrapper.SetBackgroundColor(color);
            }
        }
    }
}
