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
            var arg = e.Parameter as INode;
            if (arg != null)
            {
                e.CanExecute = arg.GroupNodeType == GroupNodeType.None;
            }
            else
            {
                var selection = NodeSelection.Manager.MainSelection;
                if (selection != null)
                {
                    e.CanExecute = selection.GroupNodeType == GroupNodeType.None;
                }
                else
                {
                    e.CanExecute = true;
                }
            }
        }

        private void CreateTorusCmd_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var parent = e.Parameter as INode;
            if (parent == null) _nodeFactory.CreateTorus();
            else _nodeFactory.CreateTorus(parent);
        }

        private void CreatePointCmd_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            var arg = e.Parameter as INode;
            if (arg != null)
            {
                e.CanExecute = arg.GroupNodeType != GroupNodeType.Fixed;
            }
            else
            {
                var selection = NodeSelection.Manager.MainSelection;
                if (selection != null)
                {
                    e.CanExecute = selection.GroupNodeType != GroupNodeType.Fixed;
                }
                else
                {
                    e.CanExecute = true;
                }
            }
        }

        private void CreatePointCmd_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var parent = e.Parameter as INode;
            if (parent == null) _nodeFactory.CreatePoint();
            else _nodeFactory.CreatePoint(parent);
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

        private void CreateBSplineCmd_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = PointsOnlySelected();
        }

        private void CreateBezierCmd_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            //_nodeFactory.CreateBezierGroupCPURenderer();
            _nodeFactory.CreateBezierGroup();
        }

        private void CreateBSplineCmd_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            _nodeFactory.CreateBSplineGroup();
        }

        private void CreateInterpolatingC2Cmd_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = PointsOnlySelected();
        }

        private void CreateInterpolatingC2Cmd_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            _nodeFactory.CreateInterpolatingC2Group();
        }

        private void CreateBezierPatchC0Cmd_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = NodeSelection.Manager.MainSelection == null;
        }

        private void CreatBezierPatchC0Cmd_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var dialog = new BezierPatchCreation();
            var result = dialog.ShowDialog();

            if (result.HasValue && result.Value == true)
            {
                _nodeFactory.CreateBezierC0Patch(dialog.PatchType, dialog.HorizontalPatches, dialog.VerticalPatches, dialog.SurfaceWidth, dialog.SurfaceHeight);
            }
        }

        private void DeleteNodeCmd_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = CanDelete(e.Parameter);
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

        private void TryDeleteSelectedCmd_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void TryDeleteSelectedCmd_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var selected = NodeSelection.Manager.SelectedNodes.ToList();
            while (selected.Count > 0)
            {
                var node = selected[selected.Count - 1];
                selected.RemoveAt(selected.Count - 1);
                if (CanDelete(node))
                {
                    node.Dispose();
                }
            }
            
        }

        private bool CanDelete(object parameter)
        {
            var depColl = parameter as IDependencyCollector;
            if (depColl != null)
            {
                return depColl.HasDependency(DependencyType.Strong) == false;
            }
            else
            {
                return true;
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
