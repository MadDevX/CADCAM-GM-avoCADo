﻿using OpenTK;
using OpenTK.Graphics;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo
{
    public class NodeFactory
    {
        private Scene _scene;
        private Cursor3D _cursor;
        private ShaderWrapper _shaderWrapper;

        public NodeFactory(Scene scene, Cursor3D cursor, ShaderWrapper shaderWrapper)
        {
            _scene = scene;
            _cursor = cursor;
            _shaderWrapper = shaderWrapper;
        }

        public INode CreateTorus()
        {
            return CreateTorus(NodeSelection.Manager.MainSelection);
        }

        public INode CreatePoint()
        {
            return CreatePoint(NodeSelection.Manager.MainSelection);
        }

        public INode CreateTorus(INode parent)
        {
            if (parent == null) parent = _scene;
            var generator = new TorusGenerator(30, 30, new TorusSurface(0.5f, 0.2f));
            var torusNode = new Node(new Transform(_cursor.Position, Vector3.Zero, Vector3.One), new MeshRenderer(_shaderWrapper, generator), NameGenerator.GenerateName(parent, "Torus"));
            parent.AttachChild(torusNode);
            return torusNode;
        }

        public INode CreatePoint(INode parent)
        {
            if (parent == null) parent = _scene;
            var pointNode = new Node(new Transform(_cursor.Position, Vector3.Zero, Vector3.One), new PointRenderer(_shaderWrapper, Color4.Yellow), NameGenerator.GenerateName(parent, "Point"));
            parent.AttachChild(pointNode);
            return pointNode;
        }

        public INode CreateBezierGroup(bool c2 = false)
        {
            var parent = _scene;
            var source = new ObservableCollection<INode>();
            ICurve curve;
            if (c2) curve = new BezierC2Curve(source);
            else curve = new BezierC0Curve(source);

            var generator = new BezierGeneratorNew(curve);
            var bezierGroup = new BezierGroupNode(source, new LineRenderer(_shaderWrapper, generator), generator, NameGenerator.GenerateName(parent, "BezierCurve"));
            var selected = NodeSelection.Manager.SelectedNodes;
            foreach(var node in selected)
            {
                if(node.Renderer is PointRenderer)
                {
                    bezierGroup.AttachChild(node);
                }
            }
            parent.AttachChild(bezierGroup);
            return bezierGroup;
        }
    }
}
