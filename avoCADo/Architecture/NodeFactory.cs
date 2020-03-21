﻿using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo
{
    public class NodeFactory
    {
        private Scene _scene;
        private Cursor3D _cursor;
        private Shader _shader;

        public NodeFactory(Scene scene, Cursor3D cursor, Shader shader)
        {
            _scene = scene;
            _cursor = cursor;
            _shader = shader;
        }

        public INode CreateTorus(INode parent)
        {
            if (parent == null) parent = _scene;
            var generator = new TorusGenerator(0.5f, 0.2f, 30, 30);
            var torusNode = new Node(new Transform(_cursor.Position, Vector3.Zero, Vector3.One), new MeshRenderer(_shader, generator), NameGenerator.GenerateName(parent, "Torus"));
            parent.AttachChild(torusNode);
            return torusNode;
        }

        public INode CreatePoint(INode parent)
        {
            if (parent == null) parent = _scene;
            var pointNode = new Node(new Transform(_cursor.Position, Vector3.Zero, Vector3.One), new PointRenderer(_shader), NameGenerator.GenerateName(parent, "Point"));
            parent.AttachChild(pointNode);
            return pointNode;
        }

        //TODO : implement
        public INode CreateBezierGroup()
        {
            return null;
        }

    }
}