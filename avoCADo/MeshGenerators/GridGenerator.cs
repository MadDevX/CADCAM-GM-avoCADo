using avoCADo.Constants;
using OpenTK;
using OpenTK.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo
{
    public class GridGenerator : IMeshGenerator
    {
        private List<DrawCall> _drawCalls = new List<DrawCall>(1);
        public IList<DrawCall> DrawCalls
        {
            get
            {
                _drawCalls.Clear();
                _drawCalls.Add(new DrawCall(0, _indices.Length, DrawCallShaderType.Default, RenderConstants.GRID_SIZE, new Color4(0.5f, 0.5f, 0.5f, 0.5f * GetPitchMultiplier()), new Color4(0.5f, 0.5f, 0.5f, 0.5f * GetPitchMultiplier())));
                return _drawCalls;
            }
        }

        public event Action OnParametersChanged;

        public int Size
        {
            get => _size;
            set
            {
                _size = Math.Min(value, _maxSize);
                UpdateData();
            }
        }

        public int Density
        {
            get => _density;
            set
            {
                _density = Math.Min(value, _maxDensity);
                UpdateData();
            }
        }

        private int _size;
        private int _maxSize = 1000;

        //how many lines per unit?
        private int _density;
        private int _maxDensity = 100;

        private ICamera _camera;

        private float[] _vertices = new float[0];
        private uint[] _indices = new uint[0];

        public GridGenerator(int size, int density, ICamera camera)
        {
            Size = size;
            Density = density;
            _camera = camera;
        }


        public void Dispose()
        {
        }

        public uint[] GetIndices()
        {
            return _indices;
        }

        public float[] GetVertices()
        {
            return _vertices;
        }

        private void UpdateData()
        {
            GenerateData();
            OnParametersChanged?.Invoke();
        }

        private void GenerateData()
        {
            var sideCount = Size * 2 + 1;
            if (_vertices.Length != sideCount * 4 * 3)
            {
                Array.Resize(ref _indices, sideCount * 4);
                Array.Resize(ref _vertices, sideCount * 4 * 3);
            }

            for (int i = 0; i < sideCount; i++)
            {
                VBOUtility.SetVertex(_vertices, new Vector3(-Size + i, 0.0f, -Size), 4 * i); //bottom side
                VBOUtility.SetVertex(_vertices, new Vector3(-Size + i, 0.0f, Size), 4 * i + 1); //top side
                VBOUtility.SetVertex(_vertices, new Vector3(Size, 0.0f, -Size + i), 4 * i + 2); //right side
                VBOUtility.SetVertex(_vertices, new Vector3(-Size, 0.0f, -Size + i), 4 * i + 3); //left side
            }

            for (uint i = 0; i < _indices.Length; i++) _indices[i] = i;
        }

        private float GetPitchMultiplier()
        {
            var minVal = 0.01f;
            var heightScale = 1.0f;
            var pitch = _camera.Pitch * _camera.Position.Y < 0.0f ? _camera.Pitch : 0.0f;
            var mult = Math.Min(1.0f, Math.Abs(pitch / ((float)Math.PI * 0.5f)) + 0.1f);
            mult *= Math.Min(1.0f, Math.Abs(_camera.Position.Y * heightScale));
            var corrected = Math.Max(mult - minVal, 0.0f) / (1.0f - minVal);
            return corrected;
        }

        public void RefreshDataPreRender()
        {
        }
    }
}
