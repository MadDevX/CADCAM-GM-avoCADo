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
        public static readonly DependencyProperty TransformProperty = DependencyProperty.Register("Transform", typeof(Transform), typeof(TransformView), new PropertyMetadata(null));
        public Transform Transform
        {
            get { return (Transform)this.GetValue(TransformProperty); }
            set
            {
                this.SetValue(TransformProperty, value);
                UpdateValues();
            }
        }

        private Dictionary<object, Action<float>> _actionDictionary = new Dictionary<object, Action<float>>();

        private bool _handleInput = true;

        public TransformView()
        {
            InitializeComponent();
            SetupDictionary();
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
            _actionDictionary.Add(posXTextBox, (x) => Transform.position.X = x);
            _actionDictionary.Add(posYTextBox, (x) => Transform.position.Y = x);
            _actionDictionary.Add(posZTextBox, (x) => Transform.position.Z = x);
            _actionDictionary.Add(sclXTextBox, (x) => Transform.scale.X = x);
            _actionDictionary.Add(sclYTextBox, (x) => Transform.scale.Y = x);
            _actionDictionary.Add(sclZTextBox, (x) => Transform.scale.Z = x);
            _actionDictionary.Add(rotXTextBox, UpdateRotation);
            _actionDictionary.Add(rotYTextBox, UpdateRotation);
            _actionDictionary.Add(rotZTextBox, UpdateRotation);

        }

        private void UpdateRotation(float arg)
        {
            var xE = MathHelper.DegreesToRadians(Convert(rotXTextBox.Text));
            var yE = MathHelper.DegreesToRadians(Convert(rotYTextBox.Text));
            var zE = MathHelper.DegreesToRadians(Convert(rotZTextBox.Text));
            Transform.Rotation = new Vector3(xE, yE, zE);
        }

        public void UpdateValues()
        {
            _handleInput = false;
            if (posXTextBox.IsFocused == false) posXTextBox.Text = Transform.position.X.ToString(_format);
            if (posYTextBox.IsFocused == false) posYTextBox.Text = Transform.position.Y.ToString(_format);
            if (posZTextBox.IsFocused == false) posZTextBox.Text = Transform.position.Z.ToString(_format);

            if (rotXTextBox.IsFocused == false) rotXTextBox.Text = (MathHelper.RadiansToDegrees(Transform.Rotation.X)).ToString(_format);
            if (rotYTextBox.IsFocused == false) rotYTextBox.Text = (MathHelper.RadiansToDegrees(Transform.Rotation.Y)).ToString(_format);
            if (rotZTextBox.IsFocused == false) rotZTextBox.Text = (MathHelper.RadiansToDegrees(Transform.Rotation.Z)).ToString(_format);

            if (sclXTextBox.IsFocused == false) sclXTextBox.Text = Transform.scale.X.ToString(_format);
            if (sclYTextBox.IsFocused == false) sclYTextBox.Text = Transform.scale.Y.ToString(_format);
            if (sclZTextBox.IsFocused == false) sclZTextBox.Text = Transform.scale.Z.ToString(_format);
            _handleInput = true;
        }

        private void PosXTextBox_GotMouseCapture(object sender, MouseEventArgs e)
        {
            (sender as TextBox).SelectAll();
        }

        private void SclXTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            (sender as TextBox).SelectAll();
        }
    }
}
