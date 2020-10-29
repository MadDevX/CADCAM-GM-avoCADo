using avoCADo.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo.Miscellaneous
{
    public class ComponentManager : IDisposable
    {
        private INode _ownerNode;
        public IList<IRenderer> Renderers { get; } = new List<IRenderer>();
        public IList<IMComponent> Components { get; } = new List<IMComponent>();

        public ComponentManager(INode ownerNode)
        {
            _ownerNode = ownerNode;
        }

        public void Dispose()
        {
            foreach (var comp in Components)
            {
                comp.Dispose();
            }
            Components.Clear();
            Renderers.Clear();
        }

        public void AttachComponents(params IMComponent[] components)
        {
            foreach (var component in components)
            {
                component.SetOwnerNode(_ownerNode);
                Components.Add(component);
                if (component is IRenderer rend)
                {
                    Renderers.Add(rend);
                }
            }
            foreach (var component in components)
            {
                component.Initialize();
            }
        }

        public void DetachComponents(params IMComponent[] components)
        {
            foreach (var component in components)
            {
                Components.Remove(component);
                if (component is IRenderer rend)
                {
                    Renderers.Remove(rend);
                }
            }
        }

        public T GetComponent<T>() where T : MComponent
        {
            foreach (var component in Components)
            {
                if (component is T tComponent) return tComponent;
            }
            return null;
        }
    }
}
