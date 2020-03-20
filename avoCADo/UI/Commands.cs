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
            var parent = e.Parameter as INode;
            if (parent == null) parent = _scene;
            var generator = new TorusGenerator(0.5f, 0.2f, 30, 30);
            var torusNode = new Node(new Transform(Vector3.Zero, Vector3.Zero, Vector3.One), new MeshRenderer(_shader, generator), NameGenerator.GenerateName(parent, "Torus"));
            parent.AttachChild(torusNode);
        }

        private void CreatePointCmd_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CreatePointCmd_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var parent = e.Parameter as INode;
            if (parent == null) parent = _scene;
            parent.AttachChild(new Node(new Transform(Vector3.Zero, Vector3.Zero, Vector3.One), new PointRenderer(_shader), NameGenerator.GenerateName(parent, "Point")));
        }

        private void DeleteNodeCmd_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void DeleteNodeCmd_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Parameter != null)
            {
                var node = e.Parameter as Node;
                if (node != null)
                {
                    node.Dispose();
                }
            }
            else
            {
                MessageBox.Show("null param");
            }
        }
    }
}
