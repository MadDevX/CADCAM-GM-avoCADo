using avoCADo;
using avoCADo.Architecture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace avoCADo.Components
{
    public abstract class UpdatableMComponent : MComponent
    {
        private IUpdateLoop _loop;

        public UpdatableMComponent()
        {
            _loop = Registry.UpdateLoop;
        }

        public override void Initialize()
        {
            _loop.OnUpdateLoop += OnUpdateWrapper;
            base.Initialize();
        }

        public override void Dispose()
        {
            _loop.OnUpdateLoop -= OnUpdateWrapper;
            base.Dispose();
        }

        protected virtual void OnUpdate(float deltaTime) { }

        private void OnUpdateWrapper(float deltaTime)
        {
            //TODO: this is left here because (probably) if Node initialization is costly, 
            //there may be a situation where Stopwatch calls OnTick before object 
            //initialization was completed (because it runs on a different thread)
            if (_initialized == false) MessageBox.Show("Update Loop tried to execute before component initialization finished", 
                                                       "Whoopsie Daisy!", MessageBoxButton.OK, MessageBoxImage.Warning);
            if(Enabled && _initialized)
            {
                OnUpdate(deltaTime);
            }
        }
    }
}
