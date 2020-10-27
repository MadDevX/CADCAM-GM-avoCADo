﻿using avoCADo.Components;
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
        public IRenderer Renderer { get; } = null;

        public Matrix4 GlobalModelMatrix => Matrix4.Identity;

        private ObservableCollection<INode> _children = new ObservableCollection<INode>();
        public ObservableCollection<INode> Children => _children;

        public event Action<INode> OnDisposed;
        public event PropertyChangedEventHandler PropertyChanged;

        public void Assign(IRenderer renderer) => renderer.SetNode(this);

        public void Set(ITransform transform) => transform.Node = this;

        public void AttachChild(INode node) { }

        public void AttachChildAtIndex(INode node, int index) { }

        public void AttachChildRange(IList<INode> nodes) { }

        public bool DetachChild(INode node) => false;

        public void DetachChildRange(IList<INode> nodes) { }

        public void Dispose()
        {
            foreach (var comp in _components)
            {
                comp.Dispose();
            }
            _components.Clear();
        }

        public int GetChildIndex(INode node) => -1;

        public void Render(ICamera camera, Matrix4 parentMatrix) { }

        #region Components
        private IList<IMComponent> _components { get; } = new List<IMComponent>();
        public void AttachComponents(params IMComponent[] components)
        {
            foreach (var component in components)
            {
                component.SetOwnerNode(this);
                _components.Add(component);
            }
            foreach (var component in _components)
            {
                component.Initialize();
            }
        }

        public T GetComponent<T>() where T : MComponent
        {
            foreach (var component in _components)
            {
                if (component is T tComponent) return tComponent;
            }
            return null;
        }
        #endregion
    }
}
