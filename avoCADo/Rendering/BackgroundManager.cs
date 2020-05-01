using OpenTK.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo
{
    public class BackgroundManager
    {
        private Color4 _backgroundColor;

        public event Action OnBackgroundColorChanged;

        public Color4 BackgroundColor
        {
            get
            {
                return _backgroundColor;
            }

            set
            {
                _backgroundColor = value;
                OnBackgroundColorChanged?.Invoke();
            }
        }

        public BackgroundManager(Color4 backgroundColor)
        {
            BackgroundColor = backgroundColor;
        }
    }
}
