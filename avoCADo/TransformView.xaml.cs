using avoCADo.Actions;
using avoCADo.HUD;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for TransformView.xaml
    /// </summary>
    public partial class TransformView : UserControl
    {
        private static string _format = "0.0#####";
        public static readonly DependencyProperty TransformProperty = DependencyProperty.Register("Transform", typeof(ITransform), typeof(TransformView), new PropertyMetadata(null));
        public ITransform Transform
        {
            get { return (ITransform)this.GetValue(TransformProperty); }
            set
            {
                this.SetValue(TransformProperty, value);
                UpdateValues();
            }
        }

        private IInstructionBuffer _instructionBuffer;
        private Cursor3D _cursor3D;

        private TransformationInstructionUtility _instructionUtility;
        private Dictionary<object, Action<float>> _actionDictionary = new Dictionary<object, Action<float>>();
        private bool _handleInput = true;

        public TransformView()
        {
            InitializeComponent();
            SetupDictionary();
        }

        public void Initialize(IInstructionBuffer instructionBuffer, Cursor3D cursor3D)
        {
            _instructionBuffer = instructionBuffer;
            _cursor3D = cursor3D;
            _instructionUtility = new TransformationInstructionUtility(_instructionBuffer, _cursor3D);
        }

        private float Convert(string text)
        {
            if (float.TryParse(text, out var res))
            {
                return res;
            }
            else
            {
                return 0.0f;
            }
        }

        private bool IsValidInput(string text)
        {
            return float.TryParse(text, out var result);
        }


        private void ValidateTextInput(object sender, TextCompositionEventArgs e)
        {
            var tb = sender as TextBox;
            if (e.Text == "-")
            {
                tb.Text = (Convert(tb.Text) * (-1.0f)).ToString(_format);
                e.Handled = true;
            }
            else
            {
                e.Handled = !IsValidInput(tb.Text + e.Text);
            }
        }

        private void ValidatePasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(String)))
            {
                String text = (String)e.DataObject.GetData(typeof(String));
                if (!IsValidInput(text))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }

        private void TextBoxLostFocus(object sender, RoutedEventArgs e)
        {
            var tb = sender as TextBox;
            var result = Convert(tb.Text);
            tb.Text = result.ToString(_format);
            _instructionUtility.BreakInstructions();
        }

        private void TextBoxHandleInput(object sender, TextChangedEventArgs e)
        {
            if (_handleInput)
            {
                var tb = sender as TextBox;
                if (_actionDictionary.TryGetValue(tb, out var action))
                {
                    action(Convert(tb.Text));
                }
            }
        }

        private void SetupDictionary()
        {
            _actionDictionary.Add(posXTextBox, (x) => { _instructionUtility.UpdateInstruction(TransformationMode.Local, TransformationType.Translation); Transform.Position = new Vector3(x, Transform.Position.Y, Transform.Position.Z);});
            _actionDictionary.Add(posYTextBox, (y) => { _instructionUtility.UpdateInstruction(TransformationMode.Local, TransformationType.Translation); Transform.Position = new Vector3(Transform.Position.X, y, Transform.Position.Z);});
            _actionDictionary.Add(posZTextBox, (z) => { _instructionUtility.UpdateInstruction(TransformationMode.Local, TransformationType.Translation); Transform.Position = new Vector3(Transform.Position.X, Transform.Position.Y, z);});
            _actionDictionary.Add(sclXTextBox, (x) => { _instructionUtility.UpdateInstruction(TransformationMode.Local, TransformationType.Scale);       Transform.Scale =    new Vector3(x, Transform.Scale.Y, Transform.Scale.Z);});
            _actionDictionary.Add(sclYTextBox, (y) => { _instructionUtility.UpdateInstruction(TransformationMode.Local, TransformationType.Scale);       Transform.Scale =    new Vector3(Transform.Scale.X, y, Transform.Scale.Z);});
            _actionDictionary.Add(sclZTextBox, (z) => { _instructionUtility.UpdateInstruction(TransformationMode.Local, TransformationType.Scale);       Transform.Scale =    new Vector3(Transform.Scale.X, Transform.Scale.Y, z);});
            _actionDictionary.Add(rotXTextBox, UpdateRotation);
            _actionDictionary.Add(rotYTextBox, UpdateRotation);
            _actionDictionary.Add(rotZTextBox, UpdateRotation);

        }

        private void UpdateRotation(float arg)
        {
            var xE = MathHelper.DegreesToRadians(Convert(rotXTextBox.Text));
            var yE = MathHelper.DegreesToRadians(Convert(rotYTextBox.Text));
            var zE = MathHelper.DegreesToRadians(Convert(rotZTextBox.Text));
            if (float.IsNaN(xE) == false && float.IsNaN(yE) == false && float.IsNaN(zE) == false)
            {
                _instructionUtility.UpdateInstruction(TransformationMode.Local, TransformationType.Rotation);
                Transform.RotationEulerAngles = new Vector3(xE, yE, zE);
            }
        }

        public void UpdateValues()
        {
            _handleInput = false;
            if (posXTextBox.IsFocused == false) posXTextBox.Text = Transform.Position.X.ToString(_format);
            if (posYTextBox.IsFocused == false) posYTextBox.Text = Transform.Position.Y.ToString(_format);
            if (posZTextBox.IsFocused == false) posZTextBox.Text = Transform.Position.Z.ToString(_format);

            var euler = Transform.RotationEulerAngles;
            if (rotXTextBox.IsFocused == false) rotXTextBox.Text = (MathHelper.RadiansToDegrees(euler.X)).ToString(_format);
            if (rotYTextBox.IsFocused == false) rotYTextBox.Text = (MathHelper.RadiansToDegrees(euler.Y)).ToString(_format);
            if (rotZTextBox.IsFocused == false) rotZTextBox.Text = (MathHelper.RadiansToDegrees(euler.Z)).ToString(_format);

            if (sclXTextBox.IsFocused == false) sclXTextBox.Text = Transform.Scale.X.ToString(_format);
            if (sclYTextBox.IsFocused == false) sclYTextBox.Text = Transform.Scale.Y.ToString(_format);
            if (sclZTextBox.IsFocused == false) sclZTextBox.Text = Transform.Scale.Z.ToString(_format);
            _handleInput = true;
        }

        private void PosXTextBox_GotMouseCapture(object sender, MouseEventArgs e)
        {
            (sender as TextBox).SelectAll();
            _instructionUtility.BreakInstructions();
        }

        private void SclXTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            (sender as TextBox).SelectAll();
            _instructionUtility.BreakInstructions();
        }
    }
}
