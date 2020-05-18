using OpenTK;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo
{
    public enum NodeType
    {
        Point,
        Curve,
        Surface,
        Scene,
        Virtual
    }

    public enum GroupNodeType
    {
        None,
        Attachable,
        Fixed
    }

    public interface INode : IDisposable, INotifyPropertyChanged
    {
        bool IsSelected { get; set; }
        GroupNodeType GroupNodeType { get; }
        NodeType NodeType { get; }
        event Action<INode> OnDisposed;
        string Name { get; set; }
        ITransform Transform { get; }
        IRenderer Renderer { get; }
        Matrix4 GlobalModelMatrix { get; }
        ObservableCollection<INode> Children { get; }

        void Render(Camera camera, Matrix4 parentMatrix);
        void AttachChild(INode node);
        bool DetachChild(INode node);
    }
}
