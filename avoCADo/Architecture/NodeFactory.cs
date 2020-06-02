using OpenTK;
using OpenTK.Graphics;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace avoCADo
{
    public class NodeFactory : IDisposable
    {
        private SceneManager _sceneManager;
        private Cursor3D _cursor;
        private IUpdateLoop _loop;
        private ShaderProvider _shaderProvider;

        public NodeFactory(SceneManager sceneManager, Cursor3D cursor, IUpdateLoop loop, ShaderProvider shaderProvider)
        {
            _sceneManager = sceneManager;
            _cursor = cursor;
            _loop = loop;
            _shaderProvider = shaderProvider;
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
            if (parent == null || parent.GroupNodeType == GroupNodeType.Fixed) parent = _sceneManager.CurrentScene;
            var generator = new TorusGenerator(30, 30, new TorusSurface(0.5f, 0.2f));
            var torusNode = new Node(new Transform(_cursor.Position, Vector3.Zero, Vector3.One), new ParametricObjectRenderer(_shaderProvider.SurfaceShaderBezier, _shaderProvider.SurfaceShaderDeBoor, _shaderProvider.CurveShader, _shaderProvider.DefaultShader, generator), NameGenerator.GenerateName(parent, "Torus"));
            torusNode.ObjectType = ObjectType.Torus;

            parent.AttachChild(torusNode);
            return torusNode;
        }

        public INode CreateBezierC0Patch(PatchType patchType, int horizontalPatches = 1, int verticalPatches = 1, float width = 1.0f, float height = 1.0f)
        {
            var parent = _sceneManager.CurrentScene;
            var bezierSurfCollection = new WpfObservableRangeCollection<INode>();
            var surface = new BezierC0Patch();
            var surfGen = new BezierPatchGenerator(surface, this, patchType, _cursor.Position, horizontalPatches, verticalPatches, width, height);
            var surfNode = new BezierPatchGroupNode(bezierSurfCollection, new ParametricObjectRenderer(_shaderProvider.SurfaceShaderBezier, _shaderProvider.SurfaceShaderDeBoor, _shaderProvider.CurveShader, _shaderProvider.DefaultShader, surfGen), surfGen, NameGenerator.GenerateName(parent, "BezierPatch"));
            surfNode.ObjectType = ObjectType.BezierPatchC0;

            parent.AttachChild(surfNode);
            return surfNode;
        }

        public INode CreateBezierC2Patch(PatchType patchType, int horizontalPatches = 1, int verticalPatches = 1, float width = 1.0f, float height = 1.0f)
        {
            var parent = _sceneManager.CurrentScene;
            var bezierSurfCollection = new WpfObservableRangeCollection<INode>();
            var surface = new BezierC2Patch();
            var surfGen = new BezierPatchC2Generator(surface, this, patchType, _cursor.Position, horizontalPatches, verticalPatches, width, height);
            var surfNode = new BezierPatchGroupNode(bezierSurfCollection, new ParametricObjectRenderer(_shaderProvider.SurfaceShaderBezier, _shaderProvider.SurfaceShaderDeBoor, _shaderProvider.CurveShader, _shaderProvider.DefaultShader, surfGen), surfGen, NameGenerator.GenerateName(parent, "BSplinePatch"));
            surfNode.ObjectType = ObjectType.BezierPatchC2;
            
            parent.AttachChild(surfNode);
            return surfNode;
        }

        private List<PoolableNode> _pointPool = new List<PoolableNode>(3000);

        private IList<INode> _pointsBuffer = new List<INode>(3000);

        public IList<INode> CreatePointsBatch(int count)
        {
            _pointsBuffer.Clear();
            for (int i = 0; i < count; i++) _pointsBuffer.Add(CreatePointInstance(_sceneManager.CurrentScene));
            _sceneManager.CurrentScene.AttachChildRange(_pointsBuffer);
            return _pointsBuffer;
        }

        public INode CreatePoint(INode parent)
        {
            if (parent == null || parent.GroupNodeType == GroupNodeType.Fixed) parent = _sceneManager.CurrentScene;
            var pointNode = CreatePointInstance(parent);
            parent.AttachChild(pointNode);
            return pointNode;
        }

        private INode CreatePointInstance(INode parent)
        {
            PoolableNode pointNode;

            if (_pointPool.Count == 0)
            {
                pointNode = new PoolableNode(new PointTransform(_cursor.Position, Vector3.Zero, Vector3.One), new PointRenderer(_shaderProvider.DefaultShader, Color4.Orange, Color4.Yellow), NameGenerator.GenerateName(parent, "Point"));
                pointNode.ObjectType = ObjectType.Point;
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
            var parent = _sceneManager.CurrentScene;
            var source = new WpfObservableRangeCollection<INode>();
            ICurve curve = new BezierC0Curve(source);

            var generator = new BezierGenerator(curve);
            var bezierGroup = new BezierGroupNode(source, new LineRenderer(_shaderProvider.DefaultShader, generator), generator, NameGenerator.GenerateName(parent, "BezierCurve"));
            bezierGroup.ObjectType = ObjectType.BezierCurveC0;

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
            var groupNode = CreateGeometryCurveGroup<BezierC0Curve>("BezierCurve");
            groupNode.ObjectType = ObjectType.BezierCurveC0;
            return groupNode;
        }

        public INode CreateBSplineGroup()
        {
            var groupNode = CreateGeometryCurveGroup<BezierC2Curve>("BSplineCurve");
            groupNode.ObjectType = ObjectType.BezierCurveC2;
            return groupNode;
        }

        public INode CreateInterpolatingC2Group()
        {
            var groupNode = CreateGeometryCurveGroup<InterpolatingC2Curve>("InterpolatingC2Curve");
            groupNode.ObjectType = ObjectType.InterpolatingCurve;
            return groupNode;
        }

        private BezierGeomGroupNode CreateGeometryCurveGroup<T>(string defaultName) where T : ICurve
        {
            var parent = _sceneManager.CurrentScene;
            var source = new WpfObservableRangeCollection<INode>();
            ICurve curve = CreateCurve<T>(source);

            var generator = new BezierGeneratorGeometry(curve);
            var bezierGroup = new BezierGeomGroupNode(source, new ParametricObjectRenderer(_shaderProvider.SurfaceShaderBezier, _shaderProvider.SurfaceShaderDeBoor, _shaderProvider.CurveShader, _shaderProvider.DefaultShader, generator), generator, NameGenerator.GenerateName(parent, defaultName));
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
