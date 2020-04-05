using OpenTK;
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
        private ShaderWrapper _geomShaderWrapper;

        public NodeFactory(Scene scene, Cursor3D cursor, ShaderWrapper shaderWrapper, ShaderWrapper geomShaderWrapper)
        {
            _scene = scene;
            _cursor = cursor;
            _shaderWrapper = shaderWrapper;
            _geomShaderWrapper = geomShaderWrapper;
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

        public INode CreateBezierGroup()
        {
            var parent = _scene;
            var source = new ObservableCollection<INode>();
            ICurve curve = new BezierC0Curve(source);

            var generator = new BezierGenerator(curve);
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

        public INode CreateBSplineGroup()
        {
            var parent = _scene;
            var source = new ObservableCollection<INode>();
            ICurve curve = new BezierC2Curve(source);

            var generator = new BezierGeneratorGeometry(curve);
            var bezierGroup = new BezierGeomGroupNode(source, new CurveRenderer(_geomShaderWrapper, _shaderWrapper, generator), generator, NameGenerator.GenerateName(parent, "BSplineCurve"));
            var selected = NodeSelection.Manager.SelectedNodes;
            foreach (var node in selected)
            {
                if (node.Renderer is PointRenderer)
                {
                    bezierGroup.AttachChild(node);
                }
            }
            parent.AttachChild(bezierGroup);
            return bezierGroup;
        }
    }
}
