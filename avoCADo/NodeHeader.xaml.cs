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
    /// Interaction logic for NodeHeader.xaml
    /// </summary>
    public partial class NodeHeader : UserControl
    {
        public static readonly DependencyProperty NodeProperty = DependencyProperty.Register("Node", typeof(Node), typeof(NodeHeader), new PropertyMetadata(null));

        public Node Node
        {
            get { return (Node)this.GetValue(NodeProperty); }
            set
            {
                this.SetValue(NodeProperty, value);
                DataContext = value;
            }
        }

        public NodeHeader()
        {
            InitializeComponent();
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (textBox.Visibility == Visibility.Visible)
            {
                ShowLabel();
            }
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Return)
            {
                ShowLabel();
                this.Focus();
            }
        }

        private void ShowLabel()
        {
            label.Visibility = Visibility.Visible;
            //btn.Visibility = Visibility.Visible;
            textBox.Visibility = Visibility.Hidden;
        }

        private void ShowTextBox()
        {
            label.Visibility = Visibility.Hidden;
            //btn.Visibility = Visibility.Hidden;
            textBox.Visibility = Visibility.Visible;
            textBox.Focus();
            textBox.SelectAll();
        }

        private void Label_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(e.MiddleButton == MouseButtonState.Pressed)
            {
                ShowTextBox();
            }
        }

        private void Label_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key==Key.F2)
            {
                ShowTextBox();
            }
        }

        private void Button_GotFocus(object sender, RoutedEventArgs e)
        {
            //btn.Opacity = 0.2f;
        }

        private void Button_LostFocus(object sender, RoutedEventArgs e)
        {
            //btn.Opacity = 0.0f;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            ShowTextBox();
        }
    }
}
