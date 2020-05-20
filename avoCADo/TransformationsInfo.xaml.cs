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
    /// Interaction logic for TransformationsInfo.xaml
    /// </summary>
    public partial class TransformationsInfo : UserControl, IBindingUpdatable
    {
        public TransformationsInfo()
        {
            InitializeComponent();
        }

        public void UpdateBindings()
        {
            tbCursorMode.GetBindingExpression(TextBlock.TextProperty).UpdateTarget();
            tbTransMode.GetBindingExpression(TextBlock.TextProperty).UpdateTarget();
            tbAxis.GetBindingExpression(TextBlock.TextProperty).UpdateTarget();
            tbSnapMode.GetBindingExpression(TextBlock.TextProperty).UpdateTarget();
            tbSnapValue.GetBindingExpression(TextBlock.TextProperty).UpdateTarget();
        }
    }
}
