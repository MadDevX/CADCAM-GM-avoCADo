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
    public interface INode : IDisposable, INotifyPropertyChanged
    {
        bool IsGroupNode { get; }

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
