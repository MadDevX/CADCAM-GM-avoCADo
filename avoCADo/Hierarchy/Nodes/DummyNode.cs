using avoCADo.Components;
using avoCADo.Miscellaneous;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo
{
    public class DummyNode : INode
    {
        public bool IsSelectable { get; set; } = true;
        public bool IsSelected { get; set; } = false;

        public ObjectType ObjectType => ObjectType.VirtualPoint;

        public GroupNodeType GroupNodeType => GroupNodeType.None;

        public string Name { get; set; }

        public ITransform Transform { get; } = null;
        public IList<IRenderer> Renderers => _componentManager.Renderers;

        public Matrix4 GlobalModelMatrix => Matrix4.Identity;

        private ComponentManager _componentManager;

        private ObservableCollection<INode> _children = new ObservableCollection<INode>();
        public ObservableCollection<INode> Children => _children;

        public event Action<INode> OnDisposed;
        public event PropertyChangedEventHandler PropertyChanged;

        public DummyNode()
        {
            _componentManager = new ComponentManager(this);
        }

        public void Set(ITransform transform) => transform.Node = this;

        public void AttachChild(INode node) { }

        public void AttachChildAtIndex(INode node, int index) { }

        public void AttachChildRange(IList<INode> nodes) { }

        public bool DetachChild(INode node) => false;

        public void DetachChildRange(IList<INode> nodes) { }

        public void Dispose()
        {
            _componentManager.Dispose();
        }

        public int GetChildIndex(INode node) => -1;

        public void Render(ICamera camera, Matrix4 parentMatrix) { }

        #region Components

        public void AttachComponents(params IMComponent[] components) => _componentManager.AttachComponents(components);

        public T GetComponent<T>() where T : MComponent => _componentManager.GetComponent<T>();

        #endregion
    }
}
