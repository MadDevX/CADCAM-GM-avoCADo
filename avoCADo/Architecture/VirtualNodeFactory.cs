using OpenTK;
using OpenTK.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo
{
    public class VirtualNodeFactory
    {
        private ShaderWrapper _shader;
        private SceneManager _sceneManager;

        public INode DefaultParent => _sceneManager.CurrentScene;

        public VirtualNodeFactory(ShaderWrapper shader, SceneManager sceneManager)
        {
            _shader = shader;
            _sceneManager = sceneManager;
        }

        public INode CreateVirtualPoint(Vector3 position)
        {
            var pointNode = new VirtualNode(new Transform(position, Quaternion.Identity, Vector3.One));
            pointNode.AttachComponents(new PointRenderer(_shader, Color4.Aqua, Color4.Aquamarine));
            _sceneManager.CurrentScene.AttachChild(pointNode);
            return pointNode;
        }

    }
}
