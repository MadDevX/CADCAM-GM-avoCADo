using avoCADo.Actions;
using avoCADo.Architecture;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    using Parameters = AttachToCurveInstruction.Parameters;
    /// <summary>
    /// Interaction logic for NodeHeader.xaml
    /// </summary>
    public partial class NodeHeader : UserControl
    {
        public static readonly DependencyProperty NodeProperty = DependencyProperty.Register("Node", typeof(INode), typeof(NodeHeader), new PropertyMetadata(new PropertyChangedCallback(OnPropertyChanged)));
        private static void OnPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is NodeHeader header)
            {
                header.PropertyChanged?.Invoke();
            }
        }

        public INode Node
        {
            get { return (INode)this.GetValue(NodeProperty); }
            set
            {
                this.SetValue(NodeProperty, value);
                DataContext = value;
                nodeIcon.Source = IconProvider.GetIcon(value.ObjectType);
            }
        }

        private INode _parentNode = null;

        private static SolidColorBrush _highlight = new SolidColorBrush(Color.FromArgb(255, 75, 185, 255));
        private ISelectionManager _selectionManager;
        private IInstructionBuffer _instructionBuffer;

        public event Action PropertyChanged;

        public NodeHeader()
        {
            PropertyChanged += SetIcon;
            InitializeComponent();
            _selectionManager = NodeSelection.Manager;
            _instructionBuffer = Registry.InstructionBuffer;
            _selectionManager.OnSelectionChanged += OnSelectionChanged;
            Unloaded += Dispose;
        }

        private void Dispose(object sender, RoutedEventArgs e)
        {
            PropertyChanged -= SetIcon;
            _selectionManager.OnSelectionChanged -= OnSelectionChanged;
            Unloaded -= Dispose;
        }

        private void SetIcon()
        {
            if (Node != null)
            {
                nodeIcon.Source = IconProvider.GetIcon(Node.ObjectType);
            }
        }

        private void OnSelectionChanged()
        {
            if(_selectionManager.MainSelection == Node)
            {
                this.Background = _highlight;
            }
            else if(Node.IsSelected)
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
                    _instructionBuffer.IssueInstruction<SelectionChangedInstruction, SelectionChangedInstruction.Parameters>(
                        new SelectionChangedInstruction.Parameters(new List<INode> { Node }, SelectionChangedInstruction.OperationType.ToggleSelect));
                }
                else
                {
                    _instructionBuffer.IssueInstruction<SelectionChangedInstruction, SelectionChangedInstruction.Parameters>(
                        new SelectionChangedInstruction.Parameters(new List<INode> { Node }, SelectionChangedInstruction.OperationType.Select));
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

        private void ContextMenu_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (_parentNode == null) SetParentNode();
            UpdateContextMenuOptions();
        }

        private void UpdateContextMenuOptions()
        {
            var sel = _selectionManager.MainSelection;
            if (sel != null && sel.GroupNodeType == GroupNodeType.Attachable && Node.Renderer is PointRenderer)
            {
                if( sel.Children.Contains(Node))
                {
                    menuItemDetachFromCurve.Visibility = Visibility.Visible;
                    menuItemAttachToCurve.Visibility = Visibility.Collapsed;
                }
                else
                {
                    menuItemDetachFromCurve.Visibility = Visibility.Collapsed;
                    menuItemAttachToCurve.Visibility = Visibility.Visible;
                }
            }
            else
            {
                menuItemAttachToCurve.Visibility = Visibility.Collapsed;
                menuItemDetachFromCurve.Visibility = Visibility.Collapsed;
            }

            if (Node.ObjectType == ObjectType.IntersectionCurve)
                convertToInterpolatingCurve.Visibility = Visibility.Visible;
            else 
                convertToInterpolatingCurve.Visibility = Visibility.Collapsed;
        }

        private void SetParentNode()
        {
            var currentTreeView = GetSelectedTreeViewItemParent(this);
            var parentTreeView = GetSelectedTreeViewItemParent(currentTreeView);
            if(parentTreeView != null)
            {
                var node = parentTreeView.DataContext as INode;
                if (node != null)
                {
                    _parentNode = parentTreeView.DataContext as INode;
                    SetContextMenu(_parentNode);
                }
            }
        }

        private void SetContextMenu(INode parentNode)
        {
            if(parentNode.GroupNodeType != GroupNodeType.None)
            {
                UpdateRemoveAvailability(parentNode);
                menuItemDelete.Visibility = Visibility.Collapsed;
                menuItemRemove.Visibility = Visibility.Visible;
            }
            else
            {
                menuItemDelete.Visibility = Visibility.Visible;
                menuItemRemove.Visibility = Visibility.Collapsed;
            }
        }

        private void UpdateRemoveAvailability(INode node)
        {
            var depAdder = node as IDependencyAdder;
            if(depAdder != null)
            {
                menuItemRemove.IsEnabled = depAdder.ChildrenDependencyType != DependencyType.Strong;
            }
        }

        public TreeViewItem GetSelectedTreeViewItemParent(DependencyObject obj)
        {
            DependencyObject parent = VisualTreeHelper.GetParent(obj);
            while (parent is TreeViewItem == false)
            {
                if (parent is TreeView || parent == null) return null;
                parent = VisualTreeHelper.GetParent(parent);
            }

            return parent as TreeViewItem;
        }

        private void MenuItemRemove_Click(object sender, RoutedEventArgs e)
        {
            _instructionBuffer.IssueInstruction<DetachFromCurveInstruction, Parameters>(new Parameters(Node, _parentNode));
        }

        private void MenuItemAttachToCurve_Click(object sender, RoutedEventArgs e)
        {
            _instructionBuffer.IssueInstruction<AttachToCurveInstruction, Parameters>(new Parameters(Node, _selectionManager.MainSelection));
        }
        private void MenuItemDetachFromCurve_Click(object sender, RoutedEventArgs e)
        {
            _instructionBuffer.IssueInstruction<DetachFromCurveInstruction, Parameters>(new Parameters(Node, _selectionManager.MainSelection));
        }

        private void convertToInterpolatingCurve_Click(object sender, RoutedEventArgs e)
        {
            var nodeFactory = Registry.NodeFactory;

            var gen = (Node.Renderer.GetGenerator() as BezierGeneratorGeometry);
            var cps = gen.Curve.ControlPoints;

            var nodes = new List<INode>(nodeFactory.CreatePointsBatch(cps.Count));
            for(int i = 0; i < cps.Count; i++)
            {
                nodes[i].Transform.WorldPosition = cps[i];
            }

            _instructionBuffer.IssueInstruction<NodeCreatedInstruction, NodeCreatedInstruction.Parameters>(
                new NodeCreatedInstruction.Parameters(nodeFactory, ObjectType.InterpolatingCurve, new CurveParameters(nodes)));
            _instructionBuffer.Clear();
        }
    }
}
