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
    public struct PatchParameters
    {
        public WrapMode patchType;
        public int horizontalPatches;// = 1
        public int verticalPatches;// = 1 
        public float width;// = 1.0f
        public float height;// = 1.0f
        public CoordList<INode> existingNodes;

        public PatchParameters(WrapMode patchType, int horizontalPatches = 1, int verticalPatches = 1, float width = 1.0f, float height = 1.0f, CoordList<INode> existingNodes = null)
        {
            this.patchType = patchType;
            this.horizontalPatches = horizontalPatches;
            this.verticalPatches = verticalPatches;
            this.width = width;
            this.height = height;
            this.existingNodes = existingNodes;
        }
    }

    public struct CurveParameters
    {
        public List<INode> controlPoints;

        public CurveParameters(List<INode> controlPoints = null)
        {
            this.controlPoints = controlPoints;
        }
    }

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

        public INode CreateObject(ObjectType type, object parameters)
        {
            switch (type)
            {
                case ObjectType.Point:
                    return CreatePoint((INode)parameters);
                case ObjectType.Torus:
                    return CreateTorus((INode)parameters);
                case ObjectType.BezierCurveC0:
                    return CreateBezierCurveC0((CurveParameters)parameters);
                case ObjectType.BezierCurveC2:
                    return CreateBezierCurveC2((CurveParameters)parameters);
                case ObjectType.InterpolatingCurve:
                    return CreateInterpolatingCurve((CurveParameters)parameters);
                case ObjectType.BezierPatchC0:
                    return CreateBezierPatchC0((PatchParameters)parameters);
                case ObjectType.BezierPatchC2:
                    return CreateBezierPatchC2((PatchParameters)parameters);
                default:
                    return null;
            }
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
            if (parent == null || parent.GroupNodeType == GroupNodeType.Fixed) parent = GetDefaultParent();
            var generator = new TorusGenerator(30, 30, new TorusSurface(0.5f, 0.2f));
            var torusNode = new Node(new Transform(_cursor.Position, Vector3.Zero, Vector3.One), new ParametricObjectRenderer(_shaderProvider.SurfaceShaderBezier, _shaderProvider.SurfaceShaderDeBoor, _shaderProvider.CurveShader, _shaderProvider.DefaultShader, generator), NameGenerator.GenerateName(parent, "Torus"));
            torusNode.ObjectType = ObjectType.Torus;

            parent.AttachChild(torusNode);
            return torusNode;
        }

        public INode CreateBezierPatchC0(PatchParameters parameters)
        {
            var parent = _sceneManager.CurrentScene;
            var bezierSurfCollection = new WpfObservableRangeCollection<INode>();
            var surface = new BezierC0Patch();
            BezierPatchGenerator surfGen;
            if (parameters.existingNodes == null)
            {
                surfGen = new BezierPatchGenerator(surface, this, parameters.patchType, _cursor.Position, parameters.horizontalPatches, parameters.verticalPatches, parameters.width, parameters.height);
            }
            else
            {
                surfGen = new BezierPatchGenerator(surface, this, parameters.patchType, _cursor.Position, parameters.horizontalPatches, parameters.verticalPatches, parameters.existingNodes);
            }
            var surfNode = new BezierPatchGroupNode(bezierSurfCollection, new ParametricObjectRenderer(_shaderProvider.SurfaceShaderBezier, _shaderProvider.SurfaceShaderDeBoor, _shaderProvider.CurveShader, _shaderProvider.DefaultShader, surfGen), surfGen, NameGenerator.GenerateName(parent, "BezierPatch"));
            surfNode.ObjectType = ObjectType.BezierPatchC0;

            parent.AttachChild(surfNode);
            return surfNode;
        }

        public INode CreateBezierPatchC2(PatchParameters parameters)
        {
            var parent = _sceneManager.CurrentScene;
            var bezierSurfCollection = new WpfObservableRangeCollection<INode>();
            var surface = new BezierC2Patch();
            BezierPatchC2Generator surfGen;
            if (parameters.existingNodes == null)
            {
                surfGen = new BezierPatchC2Generator(surface, this, parameters.patchType, _cursor.Position, parameters.horizontalPatches, parameters.verticalPatches, parameters.width, parameters.height);
            }
            else
            {
                surfGen = new BezierPatchC2Generator(surface, this, parameters.patchType, _cursor.Position, parameters.horizontalPatches, parameters.verticalPatches, parameters.existingNodes);
            }
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
            if (parent == null || parent.GroupNodeType == GroupNodeType.Fixed) parent = GetDefaultParent();
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

        public INode CreateBezierCurveC0(CurveParameters parameters) 
        { 
            var groupNode = CreateGeometryCurveGroup<BezierC0Curve>("BezierCurve", parameters.controlPoints);
            groupNode.ObjectType = ObjectType.BezierCurveC0;
            return groupNode;
        }

        public INode CreateBezierCurveC2(CurveParameters parameters)
        {
            var groupNode = CreateGeometryCurveGroup<BezierC2Curve>("BSplineCurve", parameters.controlPoints);
            groupNode.ObjectType = ObjectType.BezierCurveC2;
            return groupNode;
        }

        public INode CreateInterpolatingCurve(CurveParameters parameters)
        {
            var groupNode = CreateGeometryCurveGroup<InterpolatingC2Curve>("InterpolatingC2Curve", parameters.controlPoints);
            groupNode.ObjectType = ObjectType.InterpolatingCurve;
            return groupNode;
        }

        private BezierGeomGroupNode CreateGeometryCurveGroup<T>(string defaultName, List<INode> controlPoints) where T : ICurve
        {
            var parent = _sceneManager.CurrentScene;
            var source = new WpfObservableRangeCollection<INode>();
            ICurve curve = CreateCurve<T>(source);

            var generator = new BezierGeneratorGeometry(curve);
            var bezierGroup = new BezierGeomGroupNode(source, new ParametricObjectRenderer(_shaderProvider.SurfaceShaderBezier, _shaderProvider.SurfaceShaderDeBoor, _shaderProvider.CurveShader, _shaderProvider.DefaultShader, generator), generator, NameGenerator.GenerateName(parent, defaultName));
            var selected = controlPoints != null ? controlPoints.AsReadOnly() : NodeSelection.Manager.SelectedNodes;
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

        private INode GetDefaultParent()
        {
            var mainSelection = NodeSelection.Manager.MainSelection;
            if (mainSelection != null) return mainSelection;
            else return _sceneManager.CurrentScene;
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
