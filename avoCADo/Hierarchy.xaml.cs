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
        private SelectionManager _selectionManager;

        public Hierarchy()
        {
            InitializeComponent();
            _selectionManager = NodeSelection.Manager;
        }

        private void TreeView_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (sender as TreeViewItem == null)
                {
                    var container = treeView.ItemContainerGenerator.ContainerFromItemRecursive(treeView.SelectedItem);
                    if (container != null)
                    {
                        container.IsSelected = false;
                    }
                    _selectionManager.ResetSelection();
                    treeView.Focus();
                }
            }
        }
    }
}
