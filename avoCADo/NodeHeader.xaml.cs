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
        private static SolidColorBrush _highlight = new SolidColorBrush(Color.FromArgb(255, 75, 185, 255));
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
            NodeSelection.Manager.OnSelectionChanged += OnSelectionChanged;
            Unloaded += Dispose;
        }

        private void Dispose(object sender, RoutedEventArgs e)
        {
            NodeSelection.Manager.OnSelectionChanged -= OnSelectionChanged;
            Unloaded -= Dispose;
        }

        private void OnSelectionChanged()
        {
            if(NodeSelection.Manager.MainSelection == Node)
            {
                this.Background = _highlight;
            }
            else if(NodeSelection.Manager.SelectedNodes.Contains(Node))
            {
                this.Background = Brushes.DodgerBlue;
            }
            else
            {
                this.Background = Brushes.White;
            }
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
            textBox.Visibility = Visibility.Hidden;
        }

        private void ShowTextBox()
        {
            label.Visibility = Visibility.Hidden;
            textBox.Visibility = Visibility.Visible;
            textBox.Focus();
            textBox.SelectAll();
        }

        private void Label_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(e.LeftButton == MouseButtonState.Pressed)
            {
                if (Keyboard.Modifiers == ModifierKeys.Shift)
                {
                    NodeSelection.Manager.ToggleSelection(Node);
                }
                else
                {
                    NodeSelection.Manager.Select(Node);
                }
            }
        }

        private void Label_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key==Key.F2)
            {
                ShowTextBox();
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            ShowTextBox();
        }
    }
}
