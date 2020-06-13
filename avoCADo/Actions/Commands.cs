using avoCADo.Actions;
using avoCADo.Miscellaneous;
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
        #region Create Commands

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

        private void CreateBezierCmd_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            //_nodeFactory.CreateBezierGroupCPURenderer();
            _instructionBuffer.IssueInstruction<NodeCreatedInstruction, NodeCreatedInstruction.Parameters>(
                new NodeCreatedInstruction.Parameters(_nodeFactory, ObjectType.BezierCurveC0, new CurveParameters(NodeSelection.Manager.SelectedNodes)));
        }

        private void CreateBSplineCmd_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = PointsOnlySelected();
        }

        private void CreateBSplineCmd_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            _instructionBuffer.IssueInstruction<NodeCreatedInstruction, NodeCreatedInstruction.Parameters>(
                new NodeCreatedInstruction.Parameters(_nodeFactory, ObjectType.BezierCurveC2, new CurveParameters(NodeSelection.Manager.SelectedNodes)));
        }

        private void CreateInterpolatingC2Cmd_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = PointsOnlySelected();
        }

        private void CreateInterpolatingC2Cmd_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            _instructionBuffer.IssueInstruction<NodeCreatedInstruction, NodeCreatedInstruction.Parameters>(
                new NodeCreatedInstruction.Parameters(_nodeFactory, ObjectType.InterpolatingCurve, new CurveParameters(NodeSelection.Manager.SelectedNodes)));
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
            dialog.HorizontalPatches = 1;
            dialog.VerticalPatches = 1;
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

        #endregion

        #region Delete Commands

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
                    _instructionBuffer.IssueInstruction<NodeDeletedInstruction, NodeDeletedInstruction.Parameters>(
                        new NodeDeletedInstruction.Parameters(new List<INode> { node }));
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
            var toDelete = new List<INode>();
            while (selected.Count > 0)
            {
                var node = selected[selected.Count - 1];
                selected.RemoveAt(selected.Count - 1);
                if (CanDelete(node))
                {
                    toDelete.Add(node);
                }
            }
            if (toDelete.Count > 0)
            {
                _instructionBuffer.IssueInstruction<NodeDeletedInstruction, NodeDeletedInstruction.Parameters>(
                    new NodeDeletedInstruction.Parameters(toDelete));
            }
            
        }

        #endregion

        #region Scene Management Commands

        private void LoadSceneCmd_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void LoadSceneCmd_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var prevScene = _sceneManager.CurrentScene;
            try
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.ShowDialog();
                if (ofd.FileName != "")
                {
                    _instructionBuffer.IssueInstruction<SelectionChangedInstruction, SelectionChangedInstruction.Parameters>(
                        new SelectionChangedInstruction.Parameters(null, SelectionChangedInstruction.OperationType.Reset));
                    var result = SceneDeserializer.Deserialize(ofd.FileName);
                    _sceneManager.CreateAndSet(NameGenerator.DiscardPath(ofd.FileName, discardExtension: true));
                    SceneDeserializer.ImportScene(result, _nodeImporter, _sceneManager.CurrentScene);
                    prevScene.Dispose();
                    NameGenerator.ResetState();
                    NameGenerator.GenerateKeywords(_sceneManager.CurrentScene);
                }
            }
            catch(Exception ex)
            {
                System.Windows.MessageBox.Show($"File corrupted.\n Message: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                if(_sceneManager.CurrentScene != prevScene)
                {
                    var invalidScene = _sceneManager.SetScene(prevScene);
                    invalidScene?.Dispose();
                    _instructionBuffer.Undo();
                }
            }

        }

        private void SaveSceneCmd_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void SaveSceneCmd_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                SaveFileDialog ofd = new SaveFileDialog();
                ofd.Filter = "XML file (*.xml)|*.xml";
                ofd.DefaultExt = "xml";
                ofd.ShowDialog();
                if (ofd.FileName != "")
                {
                    var result = SceneSerializer.Serialize(_nodeExporter, _sceneManager.CurrentScene);
                    SceneSerializer.SaveSceneTo(ofd.FileName, result);
                }
            }
            catch(Exception)
            {
                System.Windows.MessageBox.Show("Unexpected error occurred.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void NewSceneCmd_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void NewSceneCmd_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var prevScene = _sceneManager.CreateAndSet("Main");
            prevScene?.Dispose();
        }

        #endregion

        #region Various Commands

        private void MergePointsCmd_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = PointsOnlySelected() && NodeSelection.Manager.SelectedNodes.Count == 2;
        }

        private void MergePointsCmd_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var selected = NodeSelection.Manager.SelectedNodes;
            _instructionBuffer.IssueInstruction<MergePointsInstruction, MergePointsInstruction.Parameters>(
                new MergePointsInstruction.Parameters(selected.ElementAt(0), selected.ElementAt(1)));
        }

        private void FillHoleCmd_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            var selected = NodeSelection.Manager.SelectedNodes;
            e.CanExecute = ObjectTypeOnlySelected(ObjectType.BezierPatchC0) &&
                           selected.Count == 3 &&
                           LoopDetector.ValidForFilling(selected.ElementAt(0), selected.ElementAt(1), selected.ElementAt(2));
        }

        private void FillHoleCmd_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var selected = NodeSelection.Manager.SelectedNodes;
            _instructionBuffer.IssueInstruction<MergePointsInstruction, MergePointsInstruction.Parameters>(
                new MergePointsInstruction.Parameters(selected.ElementAt(0), selected.ElementAt(1)));
        }

        #endregion

        #region Helper Methods

        private bool CanDelete(object parameter)
        {
            if (parameter is VirtualNode)
            {
                return false;
            }
            else if (parameter is IDependencyCollector depColl)
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
                if (node.ObjectType != ObjectType.Point)
                {
                    return false;
                }
            }
            return true;
        }

        private bool ObjectTypeOnlySelected(ObjectType type)
        {
            foreach(var node in NodeSelection.Manager.SelectedNodes)
            {
                if(node.ObjectType != type)
                {
                    return false;
                }
            }
            return true;
        }

        #endregion
    }
}
