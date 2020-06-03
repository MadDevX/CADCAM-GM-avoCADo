using avoCADo.Actions;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace avoCADo
{
    public class ScreenSelectionHandler : IDisposable
    {
        public enum HandlingState
        {
            Ready,
            OnHold,
            InProgress
        }
        public HandlingState State { get; private set; } = HandlingState.Ready;
        public Point StartLocation { get; private set; }
        public Point EndLocation { get; private set; }
        public float SingleSelectionThreshold { get; } = 20.0f;
        public float CurrentSelectionDistance
        {
            get
            {
                var deltaX = EndLocation.X - StartLocation.X;
                var deltaY = EndLocation.Y - StartLocation.Y;
                var dist = (float)Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
                return dist;
            }
        }

        private readonly GLControl _control;
        private readonly Camera _camera;
        private readonly SceneManager _sceneManager;
        private readonly IInstructionBuffer _instructionBuffer;
        private readonly ISelectionManager _selectionManager;
        private readonly float _selectionThreshold;

        /// <summary>
        /// Usage: replace element at index [0]
        /// </summary>
        private IList<INode> _singularSelectionBuffer = new List<INode>(1);

        public ScreenSelectionHandler(GLControl control, Camera camera, SceneManager sceneManager, IInstructionBuffer instructionBuffer, float selectionThreshold = 0.2f)
        {
            _control = control;
            _camera = camera;
            _sceneManager = sceneManager;
            _instructionBuffer = instructionBuffer;
            _selectionManager = NodeSelection.Manager;
            _selectionThreshold = selectionThreshold;
            _singularSelectionBuffer.Add(null);
            Initialize();
        }

        private void Initialize()
        {
            _control.MouseDown += SelectionOnMouseDown;
            _control.MouseUp += SelectionOnMouseUp;
            _control.MouseMove += SelectionOnMouseMove;
            _control.GotFocus += IgnoreFirstClick;
        }

        public void Dispose()
        {
            _control.MouseDown -= SelectionOnMouseDown;
            _control.MouseUp -= SelectionOnMouseUp;
            _control.MouseMove -= SelectionOnMouseMove;
            _control.GotFocus -= IgnoreFirstClick;
        }

        private void IgnoreFirstClick(object sender, EventArgs e)
        {
            State = HandlingState.OnHold;
        }

        private void SelectionOnMouseDown(object sender, MouseEventArgs e)
        {
            if (State == HandlingState.OnHold) { State = HandlingState.Ready; return; }
            if (State == HandlingState.Ready && e.Button == MouseButtons.Left && System.Windows.Input.Keyboard.Modifiers != System.Windows.Input.ModifierKeys.Alt)
            {
                StartLocation = e.Location;
                State = HandlingState.InProgress;
            }
        }

        private void SelectionOnMouseMove(object sender, MouseEventArgs e)
        {
            if (State == HandlingState.OnHold) return;

            EndLocation = e.Location;
        }

        private void SelectionOnMouseUp(object sender, MouseEventArgs e)
        {
            if (State == HandlingState.InProgress)
            {
                if(CurrentSelectionDistance <= SingleSelectionThreshold)
                {
                    SingleSelection(e);
                }
                else
                {
                    MultiSelect(e);
                }
                State = HandlingState.Ready;
            }
        }

        private void SingleSelection(MouseEventArgs e)
        {
            var node = Select(e.Location);
            _singularSelectionBuffer[0] = node;
            if (System.Windows.Input.Keyboard.Modifiers == System.Windows.Input.ModifierKeys.Shift)
            {
                if (node != null)
                {
                    if (node is VirtualNode || _selectionManager.MainSelection is VirtualNode)
                    {
                        ///_selectionManager.Select(node);
                        _instructionBuffer.IssueInstruction<SelectionChangedInstruction, SelectionChangedInstruction.Parameters>(
                            new SelectionChangedInstruction.Parameters(_singularSelectionBuffer, SelectionChangedInstruction.OperationType.Select));
                    }
                    else
                    {
                        _instructionBuffer.IssueInstruction<SelectionChangedInstruction, SelectionChangedInstruction.Parameters>(
                            new SelectionChangedInstruction.Parameters(_singularSelectionBuffer, SelectionChangedInstruction.OperationType.ToggleSelect));
                    }
                }
            }
            else
            {
                if (node != null)
                {
                    _instructionBuffer.IssueInstruction<SelectionChangedInstruction, SelectionChangedInstruction.Parameters>(
                               new SelectionChangedInstruction.Parameters(_singularSelectionBuffer, SelectionChangedInstruction.OperationType.Select));
                }
                else
                {
                    _instructionBuffer.IssueInstruction<SelectionChangedInstruction, SelectionChangedInstruction.Parameters>(
                            new SelectionChangedInstruction.Parameters(_singularSelectionBuffer, SelectionChangedInstruction.OperationType.Reset));
                }
            }
        }

        private void MultiSelect(MouseEventArgs e)
        {
            var minX = Math.Min(StartLocation.X, EndLocation.X);
            var maxX = Math.Max(StartLocation.X, EndLocation.X);
            var minY = Math.Min(StartLocation.Y, EndLocation.Y);
            var maxY = Math.Max(StartLocation.Y, EndLocation.Y);
            var selectionRect = new Rectangle(minX, minY, maxX - minX, maxY - minY);
            var nodesInsideRect = Select(selectionRect);

            INode parent = _sceneManager.CurrentScene;
            if(System.Windows.Input.Keyboard.Modifiers == System.Windows.Input.ModifierKeys.Shift)
            {
                _instructionBuffer.IssueInstruction<SelectionChangedInstruction, SelectionChangedInstruction.Parameters>(
                    new SelectionChangedInstruction.Parameters(nodesInsideRect, SelectionChangedInstruction.OperationType.ToggleSelect, true));
            }
            else
            {
                _instructionBuffer.IssueInstruction<SelectionChangedInstruction, SelectionChangedInstruction.Parameters>(
                    new SelectionChangedInstruction.Parameters(nodesInsideRect, SelectionChangedInstruction.OperationType.Select, true));
            }
        }


        private List<INode> _selectionBuffer = new List<INode>(3000);

        private List<INode> Select(Rectangle rect)
        {
            _selectionBuffer.Clear();
            TraverseCollection(_sceneManager.CurrentScene.Children, rect, _selectionBuffer);
            return _selectionBuffer;
        }

        private void TraverseCollection(IList<INode> sourceList, Rectangle rect, IList<INode> selectionList, bool ignoreVirtualNodes = true)
        {
            foreach (var node in sourceList)
            {
                if (ignoreVirtualNodes && node.ObjectType == ObjectType.VirtualPoint) continue;
                if (selectionList.Count > 0 && node.Transform.ParentNode != selectionList[0].Transform.ParentNode) continue;
                if (IsNodeInsideRect(node, rect))
                {
                    selectionList.Add(node);
                }
                if (node.GroupNodeType == GroupNodeType.None)
                {
                    TraverseCollection(node.Children, rect, selectionList, ignoreVirtualNodes);
                }
            }
        }

        private INode Select(Point location)
        {
            float curDist = float.MaxValue;
            INode curSelect = null;
            var mousePos = PixelToNDC(location, _control);

            TraverseCollection(_sceneManager.CurrentScene.Children, mousePos, ref curDist, ref curSelect);
            TraverseCollection(_sceneManager.CurrentScene.VirtualChildren, mousePos, ref curDist, ref curSelect);
            return curSelect;
        }

        private void TraverseCollection(IList<INode> list, Vector3 mousePos, ref float curDist, ref INode curSelect)
        {
            foreach (var node in list)
            {
                CheckSelection(node, mousePos, ref curDist, ref curSelect);
                if (node.GroupNodeType == GroupNodeType.None)
                {
                    TraverseCollection(node.Children, mousePos, ref curDist, ref curSelect);
                }
            }
        }

        private void CheckSelection(INode node, Vector3 mousePosition, ref float curDist, ref INode curSelect)
        {
            var dist = CheckDistanceFromScreenCoords(_camera, mousePosition, node);
            if (dist <= _selectionThreshold && dist < curDist)
            {
                curDist = dist;
                curSelect = node;
            }
        }

        private bool IsNodeInsideRect(INode node, Rectangle rect)
        {
            var ndcCoords = node.Transform.ScreenCoords(_camera);
            var pixelCoords = NDCToPixel(ndcCoords, _control);
            return rect.Contains(pixelCoords);
        }

        private float CheckDistanceFromScreenCoords(Camera camera, Vector3 mousePosition, INode node)
        {
            var screenSpace = node.Transform.ScreenCoords(camera);
            Vector2 diff = new Vector2(mousePosition.X - screenSpace.X, mousePosition.Y - screenSpace.Y);
            return diff.Length;
        }

        public static Vector3 PixelToNDC(Point location, GLControl ctrl)
        {
            var halfX = ctrl.Width / 2;
            var halfY = ctrl.Height / 2;
            var x =  (float)(location.X - halfX) / halfX;
            var y = -(float)(location.Y - halfY) / halfY;
            return new Vector3(x, y, 0.0f);
        }

        public static Point NDCToPixel(Vector2 position, GLControl ctrl)
        {
            var x = (position.X + 1.0f) * 0.5f;
            var y = (-position.Y + 1.0f) * 0.5f;

            return new Point((int)(x * ctrl.Width), (int)(y * ctrl.Height));
        }
    }
}
