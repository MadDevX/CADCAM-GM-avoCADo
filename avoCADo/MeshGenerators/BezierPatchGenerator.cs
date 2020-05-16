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
        public IList<DrawCall> DrawCalls => new List<DrawCall>() { new DrawCall(0, GetIndices().Length, DrawCallShaderType.Surface, RenderConstants.SURFACE_SIZE) };

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


        private uint[] _indices = new uint[0];
        private float[] _vertices = new float[0];

        public uint[] GetIndices()
        {
            return _indices;
        }

        public float[] GetVertices()
        {
            return _vertices;
        }

        private void UpdateBufferDataWrapper()
        {
            UpdateBufferData();
            OnParametersChanged?.Invoke();
        }

        private void UpdateBufferData()
        {
            var cps = Surface.ControlPoints;
            if(_vertices.Length != cps.Count * 3)
            {
                Array.Resize(ref _vertices, cps.Count * 3);
                Array.Resize(ref _indices, Surface.USegments * Surface.VSegments * 16 * 2); //isolines in two directions, thus *2
            }

            for(int i = 0; i < cps.Width; i++)
            {
                for(int j = 0; j < cps.Height; j++)
                {
                    VBOUtility.SetVertex(_vertices, cps[i, j].Transform.WorldPosition, i + j * cps.Width);
                }
            }

            int indicesIdx = 0; //should be dependent on currently processed patch
            for(int vPatch = 0; vPatch < Surface.VSegments; vPatch++)
            {
                for(int uPatch = 0; uPatch < Surface.USegments; uPatch++)
                {
                    var uIdx = uPatch * 3;
                    var vIdx = vPatch * 3;
                    for(int j = vIdx; j < vIdx + 4; j++)
                    {
                        for(int i = uIdx; i < uIdx + 4; i++)
                        {
                            _indices[indicesIdx] = (uint)(i + j * cps.Width);
                            indicesIdx++;
                        }
                    }

                }
            }
            for (int vPatch = 0; vPatch < Surface.VSegments; vPatch++)
            {
                for (int uPatch = 0; uPatch < Surface.USegments; uPatch++)
                {
                    var uIdx = uPatch * 3;
                    var vIdx = vPatch * 3;
                    for (int i = uIdx; i < uIdx + 4; i++)
                    {
                        for (int j = vIdx; j < vIdx + 4; j++)
                        {
                            _indices[indicesIdx] = (uint)(i + j * cps.Width);
                            indicesIdx++;
                        }
                    }

                }
            }
        }


        #region Control Points

        private IList<INode> _controlPointNodes = new List<INode>();
        private bool _handleTransformChanges = true;

        private void UpdateControlPoints(int horizontalPatches, int verticalPatches)
        {
            var newPointsAdded = CorrectControlPointCount(horizontalPatches, verticalPatches);
            SetNewSurfaceData(horizontalPatches, verticalPatches);
            if (newPointsAdded)
            {
                PauseTransformHandling();
                SetControlPointPoisitions();
                ResumeTransformHandling();
            }
            UpdateBufferDataWrapper();
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
                    TrackControlPoint(_nodeFactory.CreatePoint());
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
            _controlPointNodes.Add(node);
            node.PropertyChanged += HandleCPTransformChanged;
        }

        private void UntrackControlPoint(INode node)
        {
            _controlPointNodes.Remove(node);
            node.PropertyChanged -= HandleCPTransformChanged;
        }

        private void HandleCPTransformChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (_handleTransformChanges)
            {
                UpdateBufferDataWrapper();
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

        #endregion
    }
}
