using avoCADo.Constants;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo
{
    public class TorusGenerator : IMeshGenerator, ISurfaceGenerator
    {
        private TorusSurface _surface;
        public ISurface Surface => _surface;
        public event Action OnParametersChanged;

        public IList<DrawCall> DrawCalls => new List<DrawCall>{new DrawCall(0, _indices.Length, DrawCallShaderType.TexturedDefault, RenderConstants.LINE_SIZE, 16, 0, 0, Vector2.Zero, Vector2.One, false, Trim, FlipTrim)};

        public int XDivisions
        { 
            get => _xDivisions;
            set
            {
                _xDivisions = MathHelper.Clamp(value, 3, _maxDivisions);
                UpdateData();
            }
        }

        public int YDivisions
        {
            get => _yDivisions;
            set
            {
                _yDivisions = MathHelper.Clamp(value, 3, _maxDivisions);
                UpdateData();
            }
        }

        private bool _flipTrim = false;
        public bool FlipTrim
        {
            get => _flipTrim && Surface.BoundingCurves.Count > 0;
            set => _flipTrim = value;
        }

        private bool _trim = false;
        public bool Trim
        {
            get => _trim && Surface.BoundingCurves.Count > 0;
            set => _trim = value;
        }

        private int _xDivisions = 3;
        private int _yDivisions = 3;

        private float[] _vertices;
        private uint[] _indices;

        private static int _maxDivisions = 100;

        public TorusGenerator(int xDivisions, int yDivisions, TorusSurface surface)
        {
            _xDivisions = xDivisions;
            _yDivisions = yDivisions;
            _surface = surface;
            Surface.ParametersChanged += UpdateData;
            GenerateData();
        }

        #region SETTERS

        private void UpdateData()
        {
            GenerateData();
            OnParametersChanged?.Invoke();
        }

        #endregion

        public float[] GetVertices()
        {
            return _vertices;
        }

        public uint[] GetIndices()
        {
            return _indices;
        }

        private void GenerateData()
        {
            GenerateVertices();
            GenerateIndices();
        }

        private void GenerateVertices()
        {
            var newLength = _xDivisions * _yDivisions * 5; //3 for position, 2 for texCoords
            if (_vertices == null || _vertices.Length != newLength)
            {
                _vertices = new float[newLength];
            }

            var uParamRange = Surface.ParameterURange;
            var vParamRange = Surface.ParameterVRange;

            for (int y = 0; y < _yDivisions; y++)
            {
                var beta = (((float)y / _yDivisions) * (vParamRange.Y - vParamRange.X)) + vParamRange.X;
                for (int x = 0; x < _xDivisions; x++)
                {
                    var alpha = (((float)x / _xDivisions) * (uParamRange.Y - uParamRange.X)) + uParamRange.X;
                    var vertex = _surface.GetVertexLocalSpace(alpha, beta);
                    var texCoords = new Vector2((float)x / _xDivisions, (float)y / _yDivisions);
                    VBOUtility.SetVertex(_vertices, vertex, texCoords, (x + y * _xDivisions));
                }
            }
        }

        private void GenerateIndices()
        {
            var xDiv = Surface.ULoop ? _xDivisions : (_xDivisions - 1);
            var yDiv = Surface.VLoop ? _yDivisions : (_yDivisions - 1);
            var newLength = _xDivisions * _yDivisions * 4;
            //if (Surface.ULoop == false) newLength -= _yDivisions; //TODO : calculate optimal array size (now there are holes in index array with pairs of 0-0 indices)
            //if (Surface.VLoop == false) newLength -= _xDivisions;


            if (_indices == null || _indices.Length != newLength)
            {
                _indices = new uint[newLength];
            }

            for (uint y = 0; y < _yDivisions; y++)
            {
                for (uint x = 0; x < _xDivisions; x++)
                {
                    uint index = (uint)(x + y * _xDivisions);

                    if (x != _xDivisions - 1)
                    {
                        _indices[index * 4 + 0] = index;
                        _indices[index * 4 + 1] = index + 1;
                    }
                    else if (Surface.ULoop)
                    {
                        _indices[index * 4 + 0] = index;
                        _indices[index * 4 + 1] = index - x;
                    }

                    if (y != _yDivisions - 1)
                    {
                        _indices[index * 4 + 2] = index;
                        _indices[index * 4 + 3] = index + (uint)_xDivisions;
                    }
                    else if (Surface.VLoop)
                    {
                        _indices[index * 4 + 2] = index;
                        _indices[index * 4 + 3] = x;
                    }
                }
            }
        }

        public void Dispose()
        {
            Surface.ParametersChanged -= UpdateData;
            Surface.TrimTexture.Dispose();
        }

        public void RefreshDataPreRender()
        {
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, Surface.TrimTexture.TextureHandle);
        }
    }
}
