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
        private IList<INode> _controlPointNodes;
        private bool _handleTransformChanges = true;
        public bool ShouldUpdateData { get; set; } = false;
        private readonly NodeFactory _nodeFactory;
        private readonly BezierPatchGenerator _generator;
        private readonly INode _parentNode;
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
            _parentNode = parentNode;
            _loop = loop;
            _loop.OnUpdateLoop += OnUpdate;
        }
        public void Dispose()
        {
            _loop.OnUpdateLoop -= OnUpdate;
            for (int i = _controlPointNodes.Count - 1; i >= 0; i--)
            {
                DisposeControlPoint(_controlPointNodes[i]);
            }
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
            var width = (3 * horizontalPatches) + 1;
            var height = (3 * verticalPatches) + 1;
            var dataWidth = GetHorizontalControlPointCount(horizontalPatches, _generator.PatchType);
            var dataHeight = GetVerticalControlPointCount(verticalPatches, _generator.PatchType);
            _generator.Surface.ControlPoints.SetData(_controlPointNodes, dataWidth, dataHeight, width, height);
        }

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
                foreach (var point in newPoints)
                {
                    TrackControlPoint(point);
                }
            }
            else
            {
                for (int j = 0; j < _generator.Surface.ControlPoints.DataHeight; j++)
                {
                    for (int i = 0; i < _generator.Surface.ControlPoints.DataWidth; i++)
                    {
                        if (i >= dataWidth || j >= dataHeight)
                        {
                            DisposeControlPoint(_generator.Surface.ControlPoints[i, j]);
                        }
                    }
                }
            }

            return shouldAddPoints;
        }

        private int GetHorizontalControlPointCount(int horizontalPatches, PatchType type)
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

        private int GetVerticalControlPointCount(int verticalPatches, PatchType type)
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

        private void TrackControlPoint(INode node)
        {
            _parentNode.AttachChild(node);
            _controlPointNodes.Add(node);
            node.PropertyChanged += HandleCPTransformChanged;
        }

        private void UntrackControlPoint(INode node)
        {
            _parentNode.DetachChild(node);
            _controlPointNodes.Remove(node);
            node.PropertyChanged -= HandleCPTransformChanged;
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
            var width = _generator.Surface.ControlPoints.Width;
            var height = _generator.Surface.ControlPoints.Height;
            for (int j = 0; j < height; j++)
            {
                for (int i = 0; i < width; i++)
                {
                    _generator.Surface.ControlPoints[i, j].Transform.WorldPosition = position + 
                        new Vector3
                        (
                            _generator.SurfaceWidthOrRadius * (float)Math.Sin(((double)i / (width - 1)) * Math.PI * 2.0), 
                            _generator.SurfaceWidthOrRadius * (float)Math.Cos(((double)i / (width - 1)) * Math.PI * 2.0),
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
