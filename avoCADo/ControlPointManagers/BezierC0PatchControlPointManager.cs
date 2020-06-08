using avoCADo.ControlPointManagers;
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
        private readonly IDependencyAdder _depAdd;

        public BezierC0PatchControlPointManager(NodeFactory nodeFactory, BezierPatchGenerator generator, INode ownerNode)
        {
            _nodeFactory = nodeFactory;
            _generator = generator;
            _node = ownerNode;
            _depAdd = _node as IDependencyAdder;
            if (_depAdd == null) throw new InvalidOperationException("Owner node does not implement IDependencyAdder!");
            Initialize();
        }

        private void Initialize()
        {
            _node.PropertyChanged += HandleCPTransformChanged;
        }

        public void Dispose()
        {
            _node.PropertyChanged -= HandleCPTransformChanged;
            var allChildren = new List<INode>(_controlPointNodes);
            DisposeControlPointsBatch(allChildren);
            allChildren.Clear();
        }


        public void UpdateControlPoints(Vector3 position, int horizontalPatches, int verticalPatches, CoordList<INode> existingNodes)
        {
            if (_controlPointNodes == null) _controlPointNodes = new List<INode>(GetHorizontalControlPointCount(horizontalPatches, _generator.WrapMode) * GetVerticalControlPointCount(verticalPatches, _generator.WrapMode));
            SetExistingControlPoints(horizontalPatches, verticalPatches, existingNodes);

            ShouldUpdateData = true;
        }

        public void UpdateControlPoints(Vector3 position, int horizontalPatches, int verticalPatches)
        {
            if (_controlPointNodes == null) _controlPointNodes = new List<INode>(GetHorizontalControlPointCount(horizontalPatches, _generator.WrapMode) * GetVerticalControlPointCount(verticalPatches, _generator.WrapMode));
            CorrectControlPointCount(horizontalPatches, verticalPatches);
            SetNewSurfaceData(horizontalPatches, verticalPatches);

            PauseTransformHandling();
            SetControlPointPoisitionsWrapper(position);
            ResumeTransformHandling();

            ShouldUpdateData = true;
        }

        private void SetControlPointPoisitionsWrapper(Vector3 position)
        {
            switch (_generator.WrapMode)
            {
                case WrapMode.None:
                    CPStartingPositionUtility.SetControlPointPoisitionsFlat(position, _generator.Surface.ControlPoints, _generator.SurfaceWidthOrRadius, _generator.SurfaceHeight);
                    break;
                case WrapMode.Column:
                    CPStartingPositionUtility.SetControlPointPoisitionsCylinderColumnWrap(position, _generator.Surface.ControlPoints, _generator.SurfaceWidthOrRadius, _generator.SurfaceHeight);
                    break;
                case WrapMode.Row:
                    CPStartingPositionUtility.SetControlPointPoisitionsCylinderRowWrap(position, _generator.Surface.ControlPoints, _generator.SurfaceWidthOrRadius, _generator.SurfaceHeight);
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
            var dataWidth = GetHorizontalControlPointCount(horizontalPatches, _generator.WrapMode);
            var dataHeight = GetVerticalControlPointCount(verticalPatches, _generator.WrapMode);
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

        private void SetExistingControlPoints(int horizontalPatches, int verticalPatches, CoordList<INode> existingCP)
        {
            var dataWidth = GetHorizontalControlPointCount(horizontalPatches, _generator.WrapMode);
            var dataHeight = GetVerticalControlPointCount(verticalPatches, _generator.WrapMode);
            var width = GetHorizontalAbstractCPCount(horizontalPatches);
            var height = GetVerticalAbstractCPCount(verticalPatches);
            var dataCount = dataWidth * dataHeight;

            if(existingCP.DataHeight != dataHeight || existingCP.DataWidth != dataWidth)
            {
                throw new InvalidOperationException("existing CP list does not match provided patch dimensions");
            }
            TrackControlPointsBatch(existingCP.RawData);
            _generator.Surface.ControlPoints.SetData(existingCP.RawData, existingCP.DataWidth, existingCP.DataHeight, width, height);
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
            var dataWidth = GetHorizontalControlPointCount(horizontalPatches, _generator.WrapMode);
            var dataHeight = GetVerticalControlPointCount(verticalPatches, _generator.WrapMode);
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

        protected virtual int GetHorizontalControlPointCount(int horizontalPatches, WrapMode type)
        {
            return type == WrapMode.Column ? 3 * horizontalPatches : (3 * horizontalPatches) + 1;
        }

        protected virtual int GetVerticalControlPointCount(int verticalPatches, WrapMode type)
        {
            return type == WrapMode.Row ? 3 * verticalPatches : (3 * verticalPatches) + 1;
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
            GetCPsWithoutDependencies(nodes);
            UntrackControlPointsBatch(nodes);
            if (_disposeNodesBuffer.Count > 0)
            {
                //TODO: RangeObservableCollection doesn't work properly for segmented data - commented line optimizes node deletion. 
                //      Suggestion: add blockEvents and FireEvents or fix DetachRange
                //_disposeNodesBuffer[0].Transform.ParentNode.DetachChildRange(_disposeNodesBuffer);
                foreach(var nodeToDispose in _disposeNodesBuffer)
                {
                    nodeToDispose.Dispose();
                }
            }
            _disposeNodesBuffer.Clear();
        }

        /// <summary>
        /// Finds nodes without dependencies in provided list, and stores them in <see cref="_disposeNodesBuffer"/>.
        /// </summary>
        /// <param name="nodes"></param>
        private void GetCPsWithoutDependencies(IList<INode> nodes)
        {
            _disposeNodesBuffer.Clear();
            foreach (var node in nodes)
            {
                var depColl = node as IDependencyCollector;
                if (depColl != null)
                {
                    if (depColl.HasDependencyOtherThan(_depAdd) == false)
                    {
                        _disposeNodesBuffer.Add(node);
                    }
                }
                else
                {
                    throw new InvalidOperationException("Object tracked by surface was not a node!");
                }
            }
        }

        private void TrackControlPointsBatch(IList<INode> nodes)
        {
            _node.AttachChildRange(nodes);
            _controlPointNodes.AddRange(nodes);
        }

        private void TrackControlPoint(INode node)
        {
            _node.AttachChild(node);
            _controlPointNodes.Add(node);
        }

        private void UntrackControlPointsBatch(IList<INode> nodes)
        {
            foreach (var node in nodes)
            {
                _controlPointNodes.Remove(node);
            }
            _node.DetachChildRange(nodes);
        }

        private void UntrackControlPoint(INode node)
        {
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
    }
}
