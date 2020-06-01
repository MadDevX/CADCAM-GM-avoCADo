using avoCADo.HUD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace avoCADo
{
    /// <summary>
    /// Interaction logic for Cursor3DInfo.xaml
    /// </summary>
    public partial class Cursor3DInfo : UserControl, IBindingUpdatable
    {
        private Cursor3D _cursor;
        public Cursor3DInfo()
        {
            InitializeComponent();
        }

        public void UpdateBindings()
        {
            tbScreenPos.GetBindingExpression(TextBlock.TextProperty).UpdateTarget();
            vec3ViewWorldPos.UpdateBindings();
        }

        public void Initialize(Cursor3D cursor)
        {
            _cursor = cursor;
            DataContext = cursor;
            vec3ViewWorldPos.DataContext = new TransformPositionWrapper(cursor.Transform);
        }
    }
}
