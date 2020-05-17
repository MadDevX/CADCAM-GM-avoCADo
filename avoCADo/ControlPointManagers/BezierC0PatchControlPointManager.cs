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
        private IList<INode> _controlPointNodes = new List<INode>();
        private bool _handleTransformChanges = true;
        public bool ShouldUpdateData { get; set; } = false;
        private readonly NodeFactory _nodeFactory;
        private readonly BezierPatchGenerator _generator;
        private readonly INode _parentNode;

        public BezierC0PatchControlPointManager(NodeFactory nodeFactory, BezierPatchGenerator generator, INode parentNode)
        {
            _nodeFactory = nodeFactory;
            _generator = generator;
            _parentNode = parentNode;
        }

        public void Dispose()
        {
            for (int i = _controlPointNodes.Count - 1; i >= 0; i--)
            {
                DisposeControlPoint(_controlPointNodes[i]);
            }
        }


        public void UpdateControlPoints(int horizontalPatches, int verticalPatches)
        {
            var newPointsAdded = CorrectControlPointCount(horizontalPatches, verticalPatches);
            SetNewSurfaceData(horizontalPatches, verticalPatches);

            PauseTransformHandling();
            SetControlPointPoisitions();
            ResumeTransformHandling();

            ShouldUpdateData = true;
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
            _generator.Surface.ControlPoints.SetData(_controlPointNodes, width, height);
        }

        /// <summary>
        /// Returns true if new, uninitialized points were added.
        /// </summary>
        /// <param name="horizontalPatches"></param>
        /// <param name="verticalPatches"></param>
        /// <returns></returns>
        private bool CorrectControlPointCount(int horizontalPatches, int verticalPatches)
        {
            var width = (3 * horizontalPatches) + 1;
            var height = (3 * verticalPatches) + 1;
            var count = width * height;
            var shouldAddPoints = count > _controlPointNodes.Count;

            if (shouldAddPoints)
            {
                while (_controlPointNodes.Count < count)
                {
                    TrackControlPoint(_nodeFactory.CreatePoint());
                }
            }
            else
            {
                for (int j = 0; j < _generator.Surface.ControlPoints.Height; j++)
                {
                    for (int i = 0; i < _generator.Surface.ControlPoints.Width; i++)
                    {
                        if (i >= width || j >= height)
                        {
                            DisposeControlPoint(_generator.Surface.ControlPoints[i, j]);
                        }
                    }
                }
            }

            return shouldAddPoints;
        }

        private void DisposeControlPoint(INode node)
        {
            var depColl = node as IDependencyCollector;
            if (depColl != null)
            {
                if (depColl.HasDependency(DependencyType.Weak))
                {
                    UntrackControlPoint(node);
                }
                else
                {
                    UntrackControlPoint(node);
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

        private void SetControlPointPoisitions()
        {
            var width = _generator.Surface.ControlPoints.Width;
            var height = _generator.Surface.ControlPoints.Height;
            for (int j = 0; j < height; j++)
            {
                for (int i = 0; i < width; i++)
                {
                    _generator.Surface.ControlPoints[i, j].Transform.WorldPosition = new Vector3(((float)i / (width - 1)) * _generator.SurfaceWidthOrRadius, 0.0f, ((float)j / (height - 1)) * _generator.SurfaceHeight);
                }
            }
        }
    }
}
