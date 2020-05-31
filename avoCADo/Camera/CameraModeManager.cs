using OpenTK;
using OpenTK.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo
{
    public class CameraModeManager
    {
        private StereoscopicCamera _camera;
        private BackgroundManager _backgroundManager;
        private Color4 _standardBackgroundColor;
        private Color4 _stereoscopicBackgroundColor;

        public CameraModeManager(StereoscopicCamera camera, 
                                 BackgroundManager manager, 
                                 Color4 standardBackgroundColor, 
                                 Color4 stereoscopicBackgroundColor)
        {
            _camera = camera;
            _backgroundManager = manager;
            _standardBackgroundColor = standardBackgroundColor;
            _stereoscopicBackgroundColor = stereoscopicBackgroundColor;
        }

        public void SetStereoscopic(bool mode)
        {
            if (mode)
            {
                _camera.Mode = StereoscopicCamera.RenderMode.Stereoscopic;
                _backgroundManager.BackgroundColor = _stereoscopicBackgroundColor;
            }
            else
            {
                _camera.Mode = StereoscopicCamera.RenderMode.Standard;
                _backgroundManager.BackgroundColor = _standardBackgroundColor;
            }
        }

        public void SetProjection(ProjectionMode mode)
        {
            _camera.ProjectionMode = mode;
        }

        public float FocusPlaneDistance
        {
            get
            {
                return _camera.FocusPlaneDistance;
            }
            set
            {
                _camera.FocusPlaneDistance = MathHelper.Clamp(value, _camera.NearPlane, float.MaxValue);
            }
        }

        public float EyeDistance
        {
            get
            {
                return _camera.EyeDistance;
            }
            set
            {
                _camera.EyeDistance = MathHelper.Clamp(value, 0.0f, float.MaxValue);
            }
        }
    }
}
