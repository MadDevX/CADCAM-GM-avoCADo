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
using System.Windows.Shapes;

namespace avoCADo
{
    /// <summary>
    /// Interaction logic for BezierPatchCreation.xaml
    /// </summary>
    public partial class BezierPatchCreation : Window
    {
        public int HorizontalPatches { get; set; } = 1;
        public int VerticalPatches { get; set; } = 1;

        public float SurfaceHeight { get; set; } = 1.0f;
        public float SurfaceWidth { get; set; } = 1.0f;

        public PatchType PatchType { get; private set; }

        public bool InputOk
        {
            get
            {
                return !Validation.GetHasError(width) &&
                       !Validation.GetHasError(height);
            }
        }

        public BezierPatchCreation()
        {
            InitializeComponent();
            DataContext = this;
            rbFlat.IsChecked = true;
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void rb_Checked(object sender, RoutedEventArgs e)
        {
            if (rbCylinder.IsChecked.HasValue && rbCylinder.IsChecked.Value)
            {
                lblCX.Visibility = Visibility.Visible;
                lblFX.Visibility = Visibility.Collapsed;
                surfaceParams.Header = "Cylinder Parameters";
                PatchType = PatchType.Cylinder;
            }
            else
            {

                lblCX.Visibility = Visibility.Collapsed;
                lblFX.Visibility = Visibility.Visible;
                surfaceParams.Header = "Flat Surface Parameters";
                PatchType = PatchType.Flat;
            }
        }

        private void UpdateValidation(object sender, RoutedEventArgs e)
        {
            (sender as TextBox).GetBindingExpression(TextBox.TextProperty).UpdateSource();
            btnOk.IsEnabled = InputOk;
        }
    }
}
