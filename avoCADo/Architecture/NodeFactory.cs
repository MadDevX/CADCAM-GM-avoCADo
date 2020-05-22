using OpenTK;
using OpenTK.Graphics;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo
{
    public class NodeFactory : IDisposable
    {
        private Scene _scene;
        private Cursor3D _cursor;
        private IUpdateLoop _loop;
        private ShaderWrapper _defaultShaderWrapper;
        private ShaderWrapper _geomShaderWrapper;
        private TesselationShaderWrapper _tesShaderWrapper;

        public NodeFactory(Scene scene, Cursor3D cursor, IUpdateLoop loop, ShaderWrapper shaderWrapper, ShaderWrapper geomShaderWrapper, TesselationShaderWrapper tesShaderWrapper)
        {
            _scene = scene;
            _cursor = cursor;
            _loop = loop;
            _defaultShaderWrapper = shaderWrapper;
            _geomShaderWrapper = geomShaderWrapper;
            _tesShaderWrapper = tesShaderWrapper;
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
            if (parent == null || parent.GroupNodeType == GroupNodeType.Fixed) parent = _scene;
            var generator = new TorusGenerator(30, 30, new TorusSurface(0.5f, 0.2f));
            var torusNode = new Node(new Transform(_cursor.Position, Vector3.Zero, Vector3.One), new MeshRenderer(_defaultShaderWrapper, generator), NameGenerator.GenerateName(parent, "Torus"));
            parent.AttachChild(torusNode);
            return torusNode;
        }

        public INode CreateBezierC0Patch(PatchType patchType, int horizontalPatches = 1, int verticalPatches = 1, float width = 1.0f, float height = 1.0f)
        {
            var parent = _scene;
            var bezierSurfCollection = new ObservableCollection<INode>();
            var surface = new BezierC0Patch();
            var surfGen = new BezierPatchGenerator(surface, this, _loop, patchType, _cursor.Position, horizontalPatches, verticalPatches, width, height);
            var surfNode = new BezierPatchGroupNode(bezierSurfCollection, new CurveRenderer(_tesShaderWrapper, _geomShaderWrapper, _defaultShaderWrapper, surfGen), surfGen, NameGenerator.GenerateName(parent, "BezierPatch"));
            parent.AttachChild(surfNode);
            return surfNode;
        }

        private List<PoolableNode> _pointPool = new List<PoolableNode>(3000);

        private IList<INode> _pointsBuffer = new List<INode>(3000);

        public IList<INode> CreatePointsBatch(int count)
        {
            _pointsBuffer.Clear();
            for (int i = 0; i < count; i++) _pointsBuffer.Add(CreatePointInstance(_scene));
            _scene.AttachChildRange(_pointsBuffer);
            return _pointsBuffer;
        }

        public INode CreatePoint(INode parent)
        {
            if (parent == null || parent.GroupNodeType == GroupNodeType.Fixed) parent = _scene;
            var pointNode = CreatePointInstance(parent);
            parent.AttachChild(pointNode);
            return pointNode;
        }

        private INode CreatePointInstance(INode parent)
        {
            PoolableNode pointNode;

            if (_pointPool.Count == 0)
            {
                pointNode = new PoolableNode(new PointTransform(_cursor.Position, Vector3.Zero, Vector3.One), new PointRenderer(_defaultShaderWrapper, Color4.Orange, Color4.Yellow), NameGenerator.GenerateName(parent, "Point"));
                pointNode.OnReturnToPool += PointNode_OnReturnToPool;
            }
            else
            {
                pointNode = _pointPool[_pointPool.Count - 1];
                _pointPool.RemoveAt(_pointPool.Count - 1);

                pointNode.Name = NameGenerator.GenerateName(parent, "Point");
                pointNode.Transform.WorldPosition = _cursor.Position;
                pointNode.Transform.Rotation = Quaternion.Identity;
                pointNode.Transform.Scale = Vector3.One;
            }
            return pointNode;
        }

        private void PointNode_OnReturnToPool(PoolableNode node)
        {
            _pointPool.Add(node);
        }

        public INode CreateBezierGroupCPURenderer() //OLD RENDERER
        {
            var parent = _scene;
            var source = new ObservableCollection<INode>();
            ICurve curve = new BezierC0Curve(source);

            var generator = new BezierGenerator(curve);
            var bezierGroup = new BezierGroupNode(source, new LineRenderer(_defaultShaderWrapper, generator), generator, NameGenerator.GenerateName(parent, "BezierCurve"));
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
            var bezierGroup = new BezierGeomGroupNode(source, new CurveRenderer(_tesShaderWrapper, _geomShaderWrapper, _defaultShaderWrapper, generator), generator, NameGenerator.GenerateName(parent, defaultName));
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

        public void Dispose()
        {
            foreach(var node in _pointPool)
            {
                node.OnReturnToPool -= PointNode_OnReturnToPool;
                node.TrueDispose();
            }
            _pointPool.Clear();
        }
    }
}
