using avoCADo.Architecture;
using avoCADo.Constants;
using avoCADo.MeshGenerators;
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
        public IReadOnlyCollection<INode> controlPoints;

        public CurveParameters(IReadOnlyCollection<INode> controlPoints)
        {
            this.controlPoints = controlPoints;
        }
    }

    public struct IntersectionCurveParameters
    {
        public ISurface p;
        public ISurface q;
        public IList<Vector4> parameterList;

        public IntersectionCurveParameters(ISurface p, ISurface q, IList<Vector4> parameterList)
        {
            this.p = p;
            this.q = q;
            this.parameterList = parameterList;
        }
    }

    public class NodeFactory
    {
        private SceneManager _sceneManager;
        private Cursor3D _cursor;
        private ShaderProvider _shaderProvider;
        private PointNodePool _pointNodePool;

        public NodeFactory(SceneManager sceneManager, Cursor3D cursor, ShaderProvider shaderProvider, PointNodePool pointNodePool)
        {
            _sceneManager = sceneManager;
            _cursor = cursor;
            _shaderProvider = shaderProvider;
            _pointNodePool = pointNodePool;
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
                case ObjectType.GregoryPatch:
                    return CreateGregoryPatch((IReadOnlyCollection<INode>)parameters);
                case ObjectType.IntersectionCurve:
                    return CreateIntersectionCurve((IntersectionCurveParameters)parameters);
                default:
                    return null;
            }
        }

        public INode CreateGregoryPatch(IReadOnlyCollection<INode> nodes)
        {
            var parent = _sceneManager.CurrentScene; 
            var generator = new GregoryPatchGenerator(nodes.ElementAt(0), nodes.ElementAt(1), nodes.ElementAt(2));
            var childCollection = new WpfObservableRangeCollection<INode>();
            var node = new GregoryPatchGroupNode(childCollection, CreateParametricObjectRenderer(generator), generator, NameGenerator.GenerateName(parent, DefaultNodeNames.GregoryPatch));
            node.ObjectType = ObjectType.GregoryPatch;

            parent.AttachChild(node);
            return node;
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
            var torusNode = new Node(new Transform(_cursor.Position, Vector3.Zero, Vector3.One), CreateParametricObjectRenderer(generator), NameGenerator.GenerateName(parent, DefaultNodeNames.Torus));
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
            var surfNode = new BezierPatchGroupNode(bezierSurfCollection, CreateParametricObjectRenderer(surfGen), surfGen, NameGenerator.GenerateName(parent, DefaultNodeNames.BezierPatchC0));
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
            var surfNode = new BezierPatchGroupNode(bezierSurfCollection, CreateParametricObjectRenderer(surfGen), surfGen, NameGenerator.GenerateName(parent, DefaultNodeNames.BezierPatchC2));
            surfNode.ObjectType = ObjectType.BezierPatchC2;
            
            parent.AttachChild(surfNode);
            return surfNode;
        }

        private IList<INode> _pointsBuffer = new List<INode>(3000);

        public IList<INode> CreatePointsBatch(int count)
        {
            _pointsBuffer.Clear();
            for (int i = 0; i < count; i++) _pointsBuffer.Add(_pointNodePool.CreatePointInstance(_sceneManager.CurrentScene, _cursor.Position));
            _sceneManager.CurrentScene.AttachChildRange(_pointsBuffer);
            return _pointsBuffer;
        }

        public INode CreatePoint(INode parent)
        {
            if (parent == null || parent.GroupNodeType == GroupNodeType.Fixed) parent = GetDefaultParent();
            var pointNode = _pointNodePool.CreatePointInstance(parent, _cursor.Position);
            parent.AttachChild(pointNode);
            return pointNode;
        }

        public INode CreateBezierCurveC0(CurveParameters parameters) 
        { 
            var groupNode = CreateGeometryCurveGroup<BezierC0Curve>(DefaultNodeNames.BezierCurveC0, parameters.controlPoints);
            groupNode.ObjectType = ObjectType.BezierCurveC0;
            return groupNode;
        }

        public INode CreateBezierCurveC2(CurveParameters parameters)
        {
            var groupNode = CreateGeometryCurveGroup<BezierC2Curve>(DefaultNodeNames.BezierCurveC2, parameters.controlPoints);
            groupNode.ObjectType = ObjectType.BezierCurveC2;
            return groupNode;
        }

        public INode CreateInterpolatingCurve(CurveParameters parameters)
        {
            var groupNode = CreateGeometryCurveGroup<InterpolatingC2Curve>(DefaultNodeNames.InterpolatingCurve, parameters.controlPoints);
            groupNode.ObjectType = ObjectType.InterpolatingCurve;
            return groupNode;
        }


        private INode CreateIntersectionCurve(IntersectionCurveParameters parameters)
        {
            var parent = _sceneManager.CurrentScene;
            IntersectionCurve curve = new IntersectionCurve(parameters.p, parameters.q, parameters.parameterList);

            var generator = new BezierGeneratorGeometry(curve);
            var childCollection = new WpfObservableRangeCollection<INode>();
            var node = new IntersectionCurveGroupNode(childCollection, CreateParametricObjectRenderer(generator), generator, curve, new Algebra.IntersectionData(parameters.p, parameters.q), NameGenerator.GenerateName(parent, DefaultNodeNames.IntersectionCurve));
            node.ObjectType = ObjectType.IntersectionCurve;

            parent.AttachChild(node);
            return node;
        }


        private BezierGeomGroupNode CreateGeometryCurveGroup<T>(string defaultName, IReadOnlyCollection<INode> controlPoints) where T : ICurve
        {
            var parent = _sceneManager.CurrentScene;
            var source = new WpfObservableRangeCollection<INode>();
            ICurve curve = CreateCurve<T>(source);

            var generator = new BezierGeneratorGeometry(curve);
            var bezierGroup = new BezierGeomGroupNode(source, CreateParametricObjectRenderer(generator), generator, NameGenerator.GenerateName(parent, defaultName));
            var selected = controlPoints != null ? controlPoints : NodeSelection.Manager.SelectedNodes;
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


        private IRenderer CreateParametricObjectRenderer(IMeshGenerator generator)
        {
            return new ParametricObjectRenderer(_shaderProvider, generator);
        }

        private INode GetDefaultParent()
        {
            var mainSelection = NodeSelection.Manager.MainSelection;
            if (mainSelection != null) return mainSelection;
            else return _sceneManager.CurrentScene;
        }

        #region Obsolete
        /// <summary>
        /// Inefficient implementation of renderer, demonstrational purposes only.
        /// </summary>
        /// <returns></returns>
        [Obsolete]
        public INode CreateBezierGroupCPURenderer() //OLD RENDERER
        {
            var parent = _sceneManager.CurrentScene;
            var source = new WpfObservableRangeCollection<INode>();
            ICurve curve = new BezierC0Curve(source);

            var generator = new BezierGenerator(curve);
            var bezierGroup = new BezierGroupNode(source, new LineRenderer(_shaderProvider.DefaultShader, generator), generator, NameGenerator.GenerateName(parent, "BezierCurve"));
            bezierGroup.ObjectType = ObjectType.BezierCurveC0;

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
        #endregion
    }
}
