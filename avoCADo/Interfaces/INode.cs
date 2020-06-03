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
    public enum GroupNodeType
    {
        None,
        Attachable,
        Fixed
    }

    public interface INode : IDisposable, INotifyPropertyChanged
    {
        event Action<INode> OnDisposed;
        bool IsSelected { get; set; }
        ObjectType ObjectType { get; }
        GroupNodeType GroupNodeType { get; }
        string Name { get; set; }
        ITransform Transform { get; }
        IRenderer Renderer { get; }
        Matrix4 GlobalModelMatrix { get; }
        ObservableCollection<INode> Children { get; }

        void Render(Camera camera, Matrix4 parentMatrix);
        void AttachChild(INode node);
        void AttachChildAtIndex(INode node, int index);
        bool DetachChild(INode node);
        void AttachChildRange(IList<INode> nodes);
        void DetachChildRange(IList<INode> nodes);
        int GetChildIndex(INode node);
    }
}
