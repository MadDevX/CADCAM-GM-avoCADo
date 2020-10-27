using avoCADo.CNC;
using avoCADo.Constants;
using OpenTK.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo.MeshGenerators
{
    public class PathGenerator : IMeshGenerator
    {
        private List<DrawCall> _drawCalls = new List<DrawCall>(1);
        public IList<DrawCall> DrawCalls
        {
            get
            {
                _drawCalls.Clear();
                _drawCalls.Add(new DrawCall(0, _indices.Length, DrawCallShaderType.Default, RenderConstants.LINE_SIZE, RenderConstants.MILL_PATH_COLOR, RenderConstants.MILL_PATH_COLOR));
                return _drawCalls;
            }
        }

        public event Action OnParametersChanged;

        private float[] _vertices;
        private uint[] _indices;

        public PathGenerator(CNCInstructionSet instructionSet)
        {
            foreach(var inst in instructionSet.Instructions)
            {
                //inst.po
            }
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

        public void RefreshDataPreRender()
        {
        }
    }
}
