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
    /// Interaction logic for Vector3View.xaml
    /// </summary>
    public partial class Vector3View : UserControl, IBindingUpdatable
    {
        public Vector3View()
        {
            InitializeComponent();
        }

        public void UpdateBindings()
        {
            var bindingX = posXTextBox.GetBindingExpression(TextBox.TextProperty);
            var bindingY = posYTextBox.GetBindingExpression(TextBox.TextProperty);
            var bindingZ = posZTextBox.GetBindingExpression(TextBox.TextProperty);

            if (posXTextBox.IsFocused == false) bindingX.UpdateTarget(); //else bindingX.UpdateSource();
            if (posYTextBox.IsFocused == false) bindingY.UpdateTarget(); //else bindingY.UpdateSource();
            if (posZTextBox.IsFocused == false) bindingZ.UpdateTarget(); //else bindingZ.UpdateSource();
        }

        private void posXTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            (sender as TextBox).SelectAll();
        }

        private void posXTextBox_GotMouseCapture(object sender, MouseEventArgs e)
        {
            (sender as TextBox).SelectAll();
        }
    }
}
