﻿using OpenTK;
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
        private Shader _shader;
        private Scene _scene;

        public INode DefaultParent => _scene;

        public VirtualNodeFactory(Shader shader, Scene scene)
        {
            _shader = shader;
            _scene = scene;
        }

        public INode CreateVirtualPoint(Vector3 position)
        {
            var pointNode = new VirtualNode(new Transform(position, Quaternion.Identity, Vector3.One), new PointRenderer(_shader, Color4.Aqua));
            _scene.AttachChild(pointNode);
            return pointNode;
        }

    }
}
