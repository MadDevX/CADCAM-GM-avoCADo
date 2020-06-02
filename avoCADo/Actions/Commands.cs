using avoCADo.Actions;
using avoCADo.Serialization;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
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
            _instructionBuffer.IssueInstruction<NodeCreatedInstruction, NodeCreatedInstruction.Parameters>
                (new NodeCreatedInstruction.Parameters(_nodeFactory, ObjectType.Torus, parent));
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
            _instructionBuffer.IssueInstruction<NodeCreatedInstruction, NodeCreatedInstruction.Parameters>
                (new NodeCreatedInstruction.Parameters(_nodeFactory, ObjectType.Point, parent));
        }

        private void CreateBezierCmd_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = PointsOnlySelected();
        }

        private void CreateBSplineCmd_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = PointsOnlySelected();
        }

        private void CreateBezierCmd_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            //_nodeFactory.CreateBezierGroupCPURenderer();
            _instructionBuffer.IssueInstruction<NodeCreatedInstruction, NodeCreatedInstruction.Parameters>(
                new NodeCreatedInstruction.Parameters(_nodeFactory, ObjectType.BezierCurveC0, null));
        }

        private void CreateBSplineCmd_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            _instructionBuffer.IssueInstruction<NodeCreatedInstruction, NodeCreatedInstruction.Parameters>(
                new NodeCreatedInstruction.Parameters(_nodeFactory, ObjectType.BezierCurveC2, null));
        }

        private void CreateInterpolatingC2Cmd_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = PointsOnlySelected();
        }

        private void CreateInterpolatingC2Cmd_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            _instructionBuffer.IssueInstruction<NodeCreatedInstruction, NodeCreatedInstruction.Parameters>(
                new NodeCreatedInstruction.Parameters(_nodeFactory, ObjectType.InterpolatingCurve, null));
        }

        private void CreateBezierPatchC0Cmd_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = NodeSelection.Manager.MainSelection == null;
        }

        private void CreateBezierPatchC2Cmd_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = NodeSelection.Manager.MainSelection == null;
        }

        private void CreatBezierPatchC0Cmd_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var dialog = new BezierPatchCreation();
            var result = dialog.ShowDialog();

            if (result.HasValue && result.Value == true)
            {
                hierarchy.CollapseHierarchy();
                var parameters = new PatchParameters(dialog.PatchType, dialog.HorizontalPatches, dialog.VerticalPatches, dialog.SurfaceWidth, dialog.SurfaceHeight);
                _instructionBuffer.IssueInstruction<NodeCreatedInstruction, NodeCreatedInstruction.Parameters>(
                    new NodeCreatedInstruction.Parameters(_nodeFactory, ObjectType.BezierPatchC0, parameters));
            }
        }

        private void CreatBezierPatchC2Cmd_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var dialog = new BezierPatchCreation();
            dialog.HorizontalPatches = 3;
            dialog.VerticalPatches = 3;
            dialog.DialogTitle = "Create Bezier Patch C2";
            var result = dialog.ShowDialog();

            if (result.HasValue && result.Value == true)
            {
                hierarchy.CollapseHierarchy();
                var parameters = new PatchParameters(dialog.PatchType, dialog.HorizontalPatches, dialog.VerticalPatches, dialog.SurfaceWidth, dialog.SurfaceHeight);
                _instructionBuffer.IssueInstruction<NodeCreatedInstruction, NodeCreatedInstruction.Parameters>(
                    new NodeCreatedInstruction.Parameters(_nodeFactory, ObjectType.BezierPatchC2, parameters));
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
                System.Windows.Forms.MessageBox.Show("null param");
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

        private void LoadSceneCmd_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void LoadSceneCmd_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.ShowDialog();
            if (ofd.FileName != "")
            {
                var result = SceneDeserializer.Deserialize(ofd.FileName);
                var prevScene = _sceneManager.CreateNew(ofd.FileName);
                SceneDeserializer.ImportScene(result, _nodeImporter, _sceneManager.CurrentScene);
                prevScene.Dispose();
            }
        }

        private void SaveSceneCmd_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void SaveSceneCmd_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            //OpenFileDialog ofd = new OpenFileDialog();
            //ofd.ShowDialog();
            //if (ofd.FileName != "")
            //{
            //    var result = SceneDeserializer.Deserialize(ofd.FileName);
            //    var newScene = SceneDeserializer.ImportScene(result, _nodeImporter, ofd.FileName);
            //    var prevScene = _sceneManager.SetScene(newScene);
            //    prevScene.Dispose();
            //}
        }

        private bool CanDelete(object parameter)
        {
            var depColl = parameter as IDependencyCollector;
            if (depColl != null)
            {
                return depColl.HasDependency(DependencyType.Strong) == false;
            }
            else if (parameter is INode node)
            {
                return node.GroupNodeType == GroupNodeType.None || NodeSelection.Manager.SelectedNodes.Count == 1; // if is group node and other nodes selected
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
