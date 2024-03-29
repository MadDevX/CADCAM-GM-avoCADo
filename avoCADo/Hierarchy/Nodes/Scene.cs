﻿using OpenTK;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace avoCADo
{
    public class Scene : IDisposable, INode
    {
        public bool IsSelectable { get; set; } = false;
        public bool IsSelected { get; set; } = false;
        public event PropertyChangedEventHandler PropertyChanged;
        public event Action<INode> OnDisposed;
        public GroupNodeType GroupNodeType => GroupNodeType.None;
        public ObjectType ObjectType => ObjectType.Scene;

        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Name)));
            }
        }
        /// <summary>
        /// Add and remove nodes by dedicated methods (AddNode and DeleteNode)
        /// </summary>
        public ObservableCollection<INode> Children => _children;
        private WpfObservableRangeCollection<INode> _children = new WpfObservableRangeCollection<INode>();

        public List<INode> VirtualChildren { get; } = new List<INode>();

        public Matrix4 GlobalModelMatrix
        {
            get
            {
                return Matrix4.Identity;
            }
        }

        public ITransform Transform { get; } = new DummyTransform();

        public IRenderer Renderer { get; } = new DummyRenderer();

        public Scene(string name)
        {
            Name = name;
            Transform.Node = this;
        }

        public void Dispose()
        {
            while(Children.Count > 0)
            {
                Children[Children.Count - 1].Dispose();
            }
            Children.Clear();
            for (int i = VirtualChildren.Count - 1; i >= 0; i--)
            {
                VirtualChildren[i].Dispose();
            }
            VirtualChildren.Clear();
            OnDisposed?.Invoke(this);
        }

        public void Render(ICamera camera)
        {
            for(int i = 0; i < Children.Count; i++)
            {
                Children[i].Render(camera, Matrix4.Identity);
            }
            for(int i = 0; i < VirtualChildren.Count; i++)
            {
                VirtualChildren[i].Render(camera, Matrix4.Identity);
            }
        }

        public void AttachChildAtIndex(INode child, int index)
        {
            if (child.Transform.ParentNode != null) throw new InvalidOperationException("Tried to attach node that has another parent");

            var childList = GetChildListType(child);

            child.Transform.ParentNode = this;
            childList.Insert(index, child);
            child.OnDisposed += HandleChildDisposed;
        }

        public void AttachChild(INode child)
        {
            if (child.Transform.ParentNode != null) throw new InvalidOperationException("Tried to attach node that has another parent");

            var childList = GetChildListType(child);

            child.Transform.ParentNode = this;
            childList.Add(child);
            child.OnDisposed += HandleChildDisposed;
        }

        public bool DetachChild(INode child)
        {
            var childList = GetChildListType(child);

            var val = childList.Remove(child);
            if (val)
            {
                child.Transform.ParentNode = null;
                child.OnDisposed -= HandleChildDisposed;
            }
            return val;
        }

        /// <summary>
        /// Children must be all Nodes or all VirtualNodes, mixing them together will result in undefined behaviour.
        /// </summary>
        /// <param name="children"></param>
        public void AttachChildRange(IList<INode> children)
        {
            for(int i = 0; i < children.Count; i++)
            {
                if (children[i].Transform.ParentNode != null) throw new InvalidOperationException("Tried to attach node that has another parent");
                children[i].Transform.ParentNode = this;
                children[i].OnDisposed += HandleChildDisposed;
            }
            if (children[0] is VirtualNode)
            {
                VirtualChildren.AddRange(children);
            }
            else
            {
                _children.AddRange(children);
            }
        }

        /// <summary>
        /// Children must be all Nodes or all VirtualNodes, mixing them together will result in undefined behaviour.
        /// </summary>
        /// <param name="children"></param>
        public void DetachChildRange(IList<INode> children)
        {
            foreach(var child in children)
            {
                if (child.Transform.ParentNode == this)
                {
                    child.Transform.ParentNode = null;
                    child.OnDisposed -= HandleChildDisposed;
                }
            }
            if (children[0] is VirtualNode)
            {
                foreach(var child in children)
                {
                    VirtualChildren.Remove(child);
                }
            }
            else
            {
                _children.RemoveRange(children);
            }
        }

        /// <summary>
        /// Determines whether to use Children or VirtualChildren collection based on child's type.
        /// </summary>
        /// <param name="child"></param>
        /// <returns></returns>
        private IList<INode> GetChildListType(INode child)
        {
            if (child is VirtualNode)
            {
                return VirtualChildren;
            }
            else
            {
                return Children;
            }
        }

        private void HandleChildDisposed(INode node)
        {
            DetachChild(node);
        }

        public void Render(ICamera camera, Matrix4 parentMatrix){}

        public int GetChildIndex(INode node)
        {
            return Children.IndexOf(node);
        }
    }
}
