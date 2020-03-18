using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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
            e.CanExecute = true;
        }

        private void CreateTorusCmd_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var generator = new TorusGenerator(0.5f, 0.2f, 30, 30);
            var torusNode = new Node(new Transform(Vector3.Zero, Quaternion.Identity, Vector3.One), new Renderer(_shader, generator), NameGenerator.GenerateName(_scene, "Torus"));
            _scene.AddNode(torusNode);
        }

        private void CreatePointCmd_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CreatePointCmd_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MessageBox.Show("Point created!");
        }
    }
}
