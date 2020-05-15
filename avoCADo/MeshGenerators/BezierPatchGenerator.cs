using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo
{
    public class BezierPatchGenerator : IMeshGenerator, IDependent<INode>
    {
        public IList<DrawCall> DrawCalls => new List<DrawCall>() { new DrawCall(0, GetIndices().Length, DrawCallShaderType.Curve, 2.0f) };

        public event Action OnParametersChanged;

        public int HorizontalPatches 
        { 
            get
            {
                return Surface.USegments;
            }
            set
            {
                UpdateControlPoints(value, VerticalPatches);
            }
        }

        public int VerticalPatches
        {
            get
            {
                return Surface.VSegments;
            }
            set
            {
                UpdateControlPoints(HorizontalPatches, value);
            }
        }

        public float SurfaceWidth { get; set; } = 1.0f;
        public float SurfaceHeight { get; set; } = 1.0f;

        public IBezierSurface Surface { get; }

        private NodeFactory _nodeFactory;

        public BezierPatchGenerator(IBezierSurface surface, NodeFactory nodeFactory, int horizontalPatches = 1, int verticalPatches = 1)
        {
            Surface = surface;
            _nodeFactory = nodeFactory;
            Initialize(horizontalPatches, verticalPatches);
        }

        private void Initialize(int horizontalPatches, int verticalPatches)
        {
            UpdateControlPoints(horizontalPatches, verticalPatches);
        }

        public void Initialize(INode node)
        {
            //add parent node (if points should be visible in hierarchy)
        }

        public void Dispose()
        {
            for(int i = _controlPointNodes.Count - 1; i >= 0; i--)
            {
                DisposeControlPoint(_controlPointNodes[i]);
            }
        }


        private uint[] placeholderIndices = new uint[0];
        private float[] placeholderVertices = new float[0];
        public uint[] GetIndices()
        {
            return placeholderIndices;
        }

        public float[] GetVertices()
        {
            return placeholderVertices;
        }


        private IList<INode> _controlPointNodes = new List<INode>();
        
        private void UpdateControlPoints(int horizontalPatches, int verticalPatches)
        {
            var newPointsAdded = CorrectControlPointCount(horizontalPatches, verticalPatches);
            SetNewSurfaceData(horizontalPatches, verticalPatches);
            if (newPointsAdded)
            {
                SetControlPointPoisitions();
            }
        }

        private void SetNewSurfaceData(int horizontalPatches, int verticalPatches)
        {
            var width = (3 * horizontalPatches) + 1;
            var height = (3 * verticalPatches) + 1;
            Surface.ControlPoints.SetData(_controlPointNodes, width, height);
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
                    _controlPointNodes.Add(_nodeFactory.CreatePoint());
                }
            }
            else
            {
                for(int j = 0; j < Surface.ControlPoints.Height; j++)
                {
                    for(int i = 0; i < Surface.ControlPoints.Width; i++)
                    {
                        if(i >= width || j >= height)
                        {
                            DisposeControlPoint(Surface.ControlPoints[i, j]);
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
                    _controlPointNodes.Remove(node);
                }
                else
                {
                    _controlPointNodes.Remove(node);
                    node.Dispose();
                }
            }
        }

        private void SetControlPointPoisitions()
        {
            var width = Surface.ControlPoints.Width;
            var height = Surface.ControlPoints.Height;
            for (int j = 0; j < height; j++)
            {
                for(int i = 0; i < width; i++)
                {
                    Surface.ControlPoints[i, j].Transform.WorldPosition = new Vector3(((float)i / (width-1)) * SurfaceWidth, 0.0f, ((float)j / (height-1)) * SurfaceHeight);
                }
            }
        }
    }
}
