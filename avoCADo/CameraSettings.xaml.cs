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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace avoCADo
{
    /// <summary>
    /// Interaction logic for CameraSettings.xaml
    /// </summary>
    public partial class CameraSettings : UserControl
    {
        public CameraSettings()
        {
            InitializeComponent();
        }

        private void radioBtnStandard_Checked(object sender, RoutedEventArgs e)
        {
            SetStereoscopic(false);
        }

        private void radioBtnStereoscopic_Checked(object sender, RoutedEventArgs e)
        {
            SetStereoscopic(true);
        }

        private void SetStereoscopic(bool value)
        {
            var manager = DataContext as StereoscopicCameraModeManager;
            if (manager != null)
            {
                manager.SetStereoscopic(value);
            }
        }
    }
}
