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
        public Cursor3DInfo()
        {
            InitializeComponent();
        }

        public void UpdateBindings()
        {
            tbScreenPos.GetBindingExpression(TextBlock.TextProperty).UpdateTarget();
            tbWorldPos.GetBindingExpression(TextBlock.TextProperty).UpdateTarget();
        }
    }
}
