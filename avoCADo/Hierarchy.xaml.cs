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
    /// Interaction logic for Hierarchy.xaml
    /// </summary>
    public partial class Hierarchy : UserControl
    {
        public Hierarchy()
        {
            InitializeComponent();
        }

        public void TreeView_ItemFocused(object sender, RoutedEventArgs e)
        {
            //var view = treeView.ItemContainerGenerator.ContainerFromItemRecursive(treeView.SelectedItem);
            if (treeView.SelectedItem.GetType() == typeof(Node))
            {
                var node = treeView.SelectedItem as Node;
                MessageBox.Show(node.Name);
            }
            //treeView.SelectedItem.GetType().Name - data
        }
    }
}
