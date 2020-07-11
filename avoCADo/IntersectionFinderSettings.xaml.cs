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
    /// Interaction logic for IntersectionFinderSettings.xaml
    /// </summary>
    public partial class IntersectionFinderSettings : Window
    {
        public bool UseCursor { get; set; } = true;
        public float KnotDistance { get; set; } = 0.1f;
        public IntersectionFinderSettings()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void UpdateValidation(object sender, RoutedEventArgs e)
        {
            (sender as TextBox).GetBindingExpression(TextBox.TextProperty).UpdateSource();
            btnOK.IsEnabled = InputOk;
        }
        public bool InputOk
        {
            get
            {
                return !Validation.GetHasError(tbKnotDistance);
            }
        }
    }
}
