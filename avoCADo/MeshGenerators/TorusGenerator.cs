using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo
{
    public class TorusGenerator : IMeshGenerator
    {
        public event Action OnParametersChanged;

        private static float _mainRadius = 1.0f;
        private static float _tubeRadius = 0.5f;

        public float R => _mainRadius;
        public float r => _tubeRadius;
        public int XDivisions => _xDivisions;
        public int YDivisions => _yDivisions;

        private int _xDivisions;
        private int _yDivisions;

        private float[] _vertices;
        private uint[] _indices;

        private static int _maxDivisions = 100;

        public TorusGenerator(float mainRadius, float tubeRadius, int xDivisions, int yDivisions)
        {
            _mainRadius = mainRadius;
            _tubeRadius = tubeRadius;
            _xDivisions = xDivisions;
            _yDivisions = yDivisions;
            GenerateData();
        }

        #region SETTERS

        public void SetXDivisions(int divisions)
        {
            _xDivisions = MathHelper.Clamp(divisions, 3, _maxDivisions);
            UpdateData();
        }

        public void SetYDivisions(int divisions)
        {
            _yDivisions = MathHelper.Clamp(divisions, 3, _maxDivisions);
            UpdateData();
        }

        public void SetMainRadius(float radius)
        {
            _mainRadius = radius;
            UpdateData();
        }

        public void SetTubeRadius(float radius)
        {
            _tubeRadius = radius;
            UpdateData();
        }

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
            _xDivisions = MathHelper.Clamp(_xDivisions, 3, _maxDivisions);
            _yDivisions = MathHelper.Clamp(_yDivisions, 3, _maxDivisions);

            var newLength = _xDivisions * _yDivisions * 3;
            if (_vertices == null || _vertices.Length != newLength)
            {
                _vertices = new float[newLength];
            }

            var fullAngle = (float)Math.PI * 2.0f;

            for (int y = 0; y < _yDivisions; y++)
            {
                var beta = ((float)y / _yDivisions) * fullAngle;
                for (int x = 0; x < _xDivisions; x++)
                {
                    var alpha = ((float)x / _xDivisions) * fullAngle;
                    var vertex = CalculateVertex(alpha, beta);
                    _vertices[3 * (x + y * _xDivisions) + 0] = vertex.X;
                    _vertices[3 * (x + y * _xDivisions) + 1] = vertex.Y;
                    _vertices[3 * (x + y * _xDivisions) + 2] = vertex.Z;
                }
            }
        }

        private void GenerateIndices()
        {
            _xDivisions = MathHelper.Clamp(_xDivisions, 3, _maxDivisions);
            _yDivisions = MathHelper.Clamp(_yDivisions, 3, _maxDivisions);

            var newLength = _xDivisions * _yDivisions * 4;
            if (_indices == null || _indices.Length != newLength)
            {
                _indices = new uint[newLength];
            }

            for (uint y = 0; y < _yDivisions; y++)
            {
                for (uint x = 0; x < _xDivisions; x++)
                {
                    uint index = (uint)(x + y * _xDivisions);
                    _indices[index * 4 + 0] = index;
                    if (x != _xDivisions - 1) _indices[index * 4 + 1] = index + 1;
                    else _indices[index * 4 + 1] = index - x;
                    _indices[index * 4 + 2] = index;
                    if (y != _yDivisions - 1) _indices[index * 4 + 3] = index + (uint)_xDivisions;
                    else _indices[index * 4 + 3] = x;
                }
            }
        }

        private Vector3 CalculateVertex(float alpha, float beta)
        {
            return new Vector3(
                (float)((R + r * Math.Cos(beta))*Math.Cos(alpha)),
                (float)(r * Math.Sin(beta)),
                (float)((R + r * Math.Cos(beta))*Math.Sin(alpha))
                );
        }
    }
}
