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
            var pointNode = new Node(new Transform(_cursor.Position, Vector3.Zero, Vector3.One), new PointRenderer(_shaderWrapper, Color4.Orange, Color4.Yellow), NameGenerator.GenerateName(parent, "Point"));
            parent.AttachChild(pointNode);
            return pointNode;
        }

        public INode CreateBezierGroupCPURenderer() //OLD RENDERER
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

        public INode CreateBezierGroup() 
        { 
            return CreateGeometryCurveGroup<BezierC0Curve>("BezierCurve"); 
        }

        public INode CreateBSplineGroup()
        {
            return CreateGeometryCurveGroup<BezierC2Curve>("BSplineCurve");
        }

        public INode CreateInterpolatingC2Group()
        {
            return CreateGeometryCurveGroup<InterpolatingC2Curve>("InterpolatingC2Curve");
        }

        private INode CreateGeometryCurveGroup<T>(string defaultName) where T : ICurve
        {
            var parent = _scene;
            var source = new ObservableCollection<INode>();
            ICurve curve = CreateCurve<T>(source);

            var generator = new BezierGeneratorGeometry(curve);
            var bezierGroup = new BezierGeomGroupNode(source, new CurveRenderer(_geomShaderWrapper, _shaderWrapper, generator), generator, NameGenerator.GenerateName(parent, defaultName));
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

        private ICurve CreateCurve<T>(IList<INode> source) where T : ICurve
        {
            if (typeof(T) == typeof(BezierC0Curve)) return new BezierC0Curve(source);
            if (typeof(T) == typeof(BezierC2Curve)) return new BezierC2Curve(source);
            if (typeof(T) == typeof(InterpolatingC2Curve)) return new InterpolatingC2Curve(source);

            else return null;
        }
    }
}
