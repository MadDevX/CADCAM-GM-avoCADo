using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo
{
    public class BezierC0PatchControlPointManager : IDisposable
    {
        private List<INode> _controlPointNodes;
        private bool _handleTransformChanges = true;
        public bool ShouldUpdateData { get; set; } = false;
        private readonly NodeFactory _nodeFactory;
        private readonly BezierPatchGenerator _generator;
        private readonly INode _node;
        private readonly IUpdateLoop _loop;

        private float _timer = 0.0f;
        private float _nextDelay = 0.0f;
        private float _delayMin = 0.05f;
        private float _delayMaxDiffMin = 0.05f;
        private Random _rand = new Random();

        public BezierC0PatchControlPointManager(NodeFactory nodeFactory, BezierPatchGenerator generator, INode parentNode, IUpdateLoop loop)
        {
            _nodeFactory = nodeFactory;
            _generator = generator;
            _node = parentNode;
            _loop = loop;
            _loop.OnUpdateLoop += OnUpdate;
        }

        public void Dispose()
        {
            _loop.OnUpdateLoop -= OnUpdate;
            var allChildren = new List<INode>(_controlPointNodes);
            DisposeControlPointsBatch(allChildren);
            allChildren.Clear();
        }


        public void UpdateControlPoints(Vector3 position, int horizontalPatches, int verticalPatches)
        {
            if (_controlPointNodes == null) _controlPointNodes = new List<INode>(GetHorizontalControlPointCount(horizontalPatches, _generator.PatchType) * GetVerticalControlPointCount(verticalPatches, _generator.PatchType));
            var newPointsAdded = CorrectControlPointCount(horizontalPatches, verticalPatches);
            SetNewSurfaceData(horizontalPatches, verticalPatches);

            PauseTransformHandling();
            SetControlPointPoisitionsWrapper(position);
            ResumeTransformHandling();

            ShouldUpdateData = true;
        }

        private void SetControlPointPoisitionsWrapper(Vector3 position)
        {
            switch (_generator.PatchType)
            {
                case PatchType.Flat:
                    SetControlPointPoisitionsFlat(position);
                    break;
                case PatchType.Cylinder:
                    SetControlPointPoisitionsCylinder(position);
                    break;
            }
        }

        private void ResumeTransformHandling()
        {
            _handleTransformChanges = true;
        }

        private void PauseTransformHandling()
        {
            _handleTransformChanges = false;
        }

        private void SetNewSurfaceData(int horizontalPatches, int verticalPatches)
        {
            var width = GetHorizontalAbstractCPCount(horizontalPatches);
            var height = GetVerticalAbstractCPCount(verticalPatches);
            var dataWidth = GetHorizontalControlPointCount(horizontalPatches, _generator.PatchType);
            var dataHeight = GetVerticalControlPointCount(verticalPatches, _generator.PatchType);
            _generator.Surface.ControlPoints.SetData(_controlPointNodes, dataWidth, dataHeight, width, height);
        }

        protected virtual int GetHorizontalAbstractCPCount(int horizontalPatches)
        {
            return (3 * horizontalPatches) + 1;
        }
        protected virtual int GetVerticalAbstractCPCount(int verticalPatches)
        {
            return (3 * verticalPatches) + 1;
        }

        /// <summary>
        /// Used by <see cref="CorrectControlPointCount(int, int)"/> while removing nodes.
        /// </summary>
        private IList<INode> _disposeCPsBuffer = new List<INode>();

        /// <summary>
        /// Returns true if new, uninitialized points were added.
        /// </summary>
        /// <param name="horizontalPatches"></param>
        /// <param name="verticalPatches"></param>
        /// <returns></returns>
        private bool CorrectControlPointCount(int horizontalPatches, int verticalPatches)
        {
            var dataWidth = GetHorizontalControlPointCount(horizontalPatches, _generator.PatchType);
            var dataHeight = GetVerticalControlPointCount(verticalPatches, _generator.PatchType);
            var dataCount = dataWidth * dataHeight;
            var shouldAddPoints = dataCount > _controlPointNodes.Count;

            if (shouldAddPoints)
            {
                int toCreate = dataCount - _controlPointNodes.Count;
                var newPoints = _nodeFactory.CreatePointsBatch(toCreate);
                TrackControlPointsBatch(newPoints);
            }
            else
            {
                _disposeCPsBuffer.Clear();
                for (int j = 0; j < _generator.Surface.ControlPoints.DataHeight; j++)
                {
                    for (int i = 0; i < _generator.Surface.ControlPoints.DataWidth; i++)
                    {
                        if (i >= dataWidth || j >= dataHeight)
                        {
                            _disposeCPsBuffer.Add(_generator.Surface.ControlPoints[i, j]);
                        }
                    }
                }
                DisposeControlPointsBatch(_disposeCPsBuffer);
                _disposeCPsBuffer.Clear();
            }

            return shouldAddPoints;
        }

        protected virtual int GetHorizontalControlPointCount(int horizontalPatches, PatchType type)
        {
            switch(type)
            {
                case PatchType.Flat:
                    return (3 * horizontalPatches) + 1;
                case PatchType.Cylinder:
                    return 3 * horizontalPatches;
                default:
                    return (3 * horizontalPatches) + 1;
            }
        }

        protected virtual int GetVerticalControlPointCount(int verticalPatches, PatchType type)
        {
            return (3 * verticalPatches) + 1;
        }

        private void DisposeControlPoint(INode node)
        {
            var depColl = node as IDependencyCollector;
            if (depColl != null)
            {
                UntrackControlPoint(node);
                if (depColl.HasDependency(DependencyType.Weak) == false)
                {
                    node.Dispose();
                }
            }
            else
            {
                throw new InvalidOperationException("Object tracked by surface was not a node!");
            }
        }

        /// <summary>
        /// Used by <see cref="DisposeControlPointsBatch(IList{INode})"/>
        /// </summary>
        private IList<INode> _disposeNodesBuffer = new List<INode>();

        private void DisposeControlPointsBatch(IList<INode> nodes)
        {
            _disposeNodesBuffer.Clear();
            foreach(var node in nodes)
            {
                var depColl = node as IDependencyCollector;
                if (depColl != null)
                {
                    if (depColl.HasDependency(DependencyType.Weak) == false)
                    {
                        _disposeNodesBuffer.Add(node);
                    }
                }
                else
                {
                    throw new InvalidOperationException("Object tracked by surface was not a node!");
                }
            }
            UntrackControlPointsBatch(nodes);
            if (_disposeNodesBuffer.Count > 0)
            {
                _disposeNodesBuffer[0].Transform.ParentNode.DetachChildRange(_disposeNodesBuffer);
            }
            _disposeNodesBuffer.Clear();
        }


        private void TrackControlPointsBatch(IList<INode> nodes)
        {
            foreach(var node in nodes)
            {
                node.PropertyChanged += HandleCPTransformChanged;
            }
            _node.AttachChildRange(nodes);
            _controlPointNodes.AddRange(nodes);
        }

        private void TrackControlPoint(INode node)
        {
            node.PropertyChanged += HandleCPTransformChanged;
            _node.AttachChild(node);
            _controlPointNodes.Add(node);
        }

        private void UntrackControlPointsBatch(IList<INode> nodes)
        {
            foreach (var node in nodes)
            {
                node.PropertyChanged -= HandleCPTransformChanged;
                _controlPointNodes.Remove(node);
            }
            _node.DetachChildRange(nodes);
        }

        private void UntrackControlPoint(INode node)
        {
            node.PropertyChanged -= HandleCPTransformChanged;
            _node.DetachChild(node);
            _controlPointNodes.Remove(node);
        }

        private void HandleCPTransformChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (_handleTransformChanges)
            {
                ShouldUpdateData = true;
            }
        }

        private void SetControlPointPoisitionsFlat(Vector3 position)
        {
            var width = _generator.Surface.ControlPoints.Width;
            var height = _generator.Surface.ControlPoints.Height;
            for (int j = 0; j < height; j++)
            {
                for (int i = 0; i < width; i++)
                {
                    _generator.Surface.ControlPoints[i, j].Transform.WorldPosition = position + new Vector3(((float)i / (width - 1)) * _generator.SurfaceWidthOrRadius, 0.0f, ((float)j / (height - 1)) * _generator.SurfaceHeight);
                }
            }
        }

        private void SetControlPointPoisitionsCylinder(Vector3 position)
        {
            var width = _generator.Surface.ControlPoints.DataWidth;
            var height = _generator.Surface.ControlPoints.DataHeight;
            for (int j = 0; j < height; j++)
            {
                for (int i = 0; i < width; i++)
                {
                    _generator.Surface.ControlPoints[i, j].Transform.WorldPosition = position + 
                        new Vector3
                        (
                            _generator.SurfaceWidthOrRadius * (float)Math.Sin(((double)i / (width)) * Math.PI * 2.0), 
                            _generator.SurfaceWidthOrRadius * (float)Math.Cos(((double)i / (width)) * Math.PI * 2.0),
                            ((float)j / (height - 1)) * _generator.SurfaceHeight
                        );
                }
            }
        }

        private void OnUpdate(float deltaTime)
        {
            _timer += deltaTime;
            if (_timer >= _nextDelay)
            {
                _timer = 0.0f;
                _nextDelay = (float)_rand.NextDouble() * _delayMaxDiffMin + _delayMin;
                ShouldUpdateData = true;
            }
        }

    }
}
