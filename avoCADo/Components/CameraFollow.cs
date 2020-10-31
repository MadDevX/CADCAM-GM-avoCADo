using avoCADo.Architecture;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo.Components
{
    public class CameraFollow : UpdatableMComponent
    {
        private ICamera _camera;
        private ISelectionManager _selectionManager;

        public CameraFollow()
        {
            _camera = Registry.Camera;
            _selectionManager = NodeSelection.Manager;
        }

        protected override void OnLateUpdate(float deltaTime)
        {
            if(Keyboard.GetState().IsKeyDown(Key.Space))
            {
                if(_selectionManager.MainSelection != null)
                {
                    _camera.Move(_selectionManager.MainSelection.Transform.WorldPosition);
                }
            }
        }
    }
}
