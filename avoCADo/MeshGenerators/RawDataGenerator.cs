using avoCADo.Constants;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics;

namespace avoCADo
{
    public class RawDataGenerator : IMeshGenerator
    {
        public DrawCallShaderType DrawCallShaderType { get; set; } = DrawCallShaderType.Curve;
        public float Size { get; set; } = RenderConstants.LINE_SIZE;
        public Color4 DefaultColor { get; set; }
        public Color4 SelectedColor { get; set; }

        public IList<DrawCall> DrawCalls => new List<DrawCall>() { new DrawCall(0, _indices.Length, DrawCallShaderType, Size, DefaultColor, SelectedColor) };

        public event Action OnParametersChanged;

        private float[] _vertices;
        private uint[] _indices;

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

        public void RefreshDataPreRender()
        {
        }

        public void SetData(IList<Vector3> vertices)
        {
            Array.Resize(ref _vertices, vertices.Count * 3);
            Array.Resize(ref _indices, vertices.Count);
            for(int i = 0; i < vertices.Count; i++)
            {
                VBOUtility.SetVertex(_vertices, vertices[i], i);
                _indices[i] = (uint)i;
            }
        }
    }
}
