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
            radioBtnPerspective.IsChecked = true;
            radioBtnStandard.IsChecked = true;
        }

        private void radioBtnStandard_Checked(object sender, RoutedEventArgs e)
        {
            SetStereoscopic(false);
            stackPanelStereo.Visibility = Visibility.Collapsed;
            stackPanelProjection.Visibility = Visibility.Visible;
        }

        private void radioBtnStereoscopic_Checked(object sender, RoutedEventArgs e)
        {
            SetStereoscopic(true);
            stackPanelStereo.Visibility = Visibility.Visible;
            stackPanelProjection.Visibility = Visibility.Collapsed;
        }

        private void SetStereoscopic(bool value)
        {
            var manager = DataContext as CameraModeManager;
            if (manager != null)
            {
                manager.SetStereoscopic(value);
            }
        }

        private void radioBtnPerspective_Checked(object sender, RoutedEventArgs e)
        {
            SetProjection(ProjectionMode.Perspective);
        }

        private void radioBtnOrthographic_Checked(object sender, RoutedEventArgs e)
        {
            SetProjection(ProjectionMode.Orthographic);
        }

        private void SetProjection(ProjectionMode mode)
        {
            var manager = DataContext as CameraModeManager;
            if (manager != null)
            {
                manager.SetProjection(mode);
            }
        }
    }
}
