using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace avoCADo
{
    /// <summary>
    /// Partial class responsible for custom command implementations.
    /// </summary>
    public partial class MainWindow
    {
        private void CreateTorusCmd_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CreateTorusCmd_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var parent = e.Parameter as INode;
            if (parent == null) _compositionRoot.NodeFactory.CreateTorus();
            else _compositionRoot.NodeFactory.CreateTorus(parent);
        }

        private void CreatePointCmd_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CreatePointCmd_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var parent = e.Parameter as INode;
            if (parent == null) _compositionRoot.NodeFactory.CreatePoint();
            else _compositionRoot.NodeFactory.CreatePoint(parent);
        }

        private void CreateBezierCmd_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            foreach(var node in NodeSelection.Manager.SelectedNodes)
            {
                if (node.Renderer is PointRenderer == false)
                {
                    e.CanExecute = false;
                    return;
                }
            }
            e.CanExecute = true;
        }

        private void CreateInterpolatingC2Cmd_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = PointsOnlySelected();
        }

        private void CreateBSplineCmd_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = PointsOnlySelected();
        }

        private void CreateBezierCmd_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            _compositionRoot.NodeFactory.CreateBezierGroupCPURenderer();
        }

        private void CreateBSplineCmd_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            _compositionRoot.NodeFactory.CreateBSplineGroup();
        }

        private void CreateInterpolatingC2Cmd_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            _compositionRoot.NodeFactory.CreateInterpolatingC2Group();
        }

        private void DeleteNodeCmd_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void DeleteNodeCmd_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Parameter != null)
            {
                var node = e.Parameter as INode;
                if (node != null)
                {
                    node.Dispose();
                }
            }
            else
            {
                MessageBox.Show("null param");
            }
        }

        private bool PointsOnlySelected()
        {
            foreach (var node in NodeSelection.Manager.SelectedNodes)
            {
                if (node.Renderer is PointRenderer == false)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
