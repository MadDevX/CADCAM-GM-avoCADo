using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo.MeshGenerators
{
    public class GregoryPatchGenerator : IMeshGenerator
    {
        private List<DrawCall> _drawCalls = new List<DrawCall>(3);
        public IList<DrawCall> DrawCalls
        {
            get
            {
                ////_drawCalls.Clear();
                ////if(ShowEdges)
                ////{
                ////    _drawCalls.Add(new DrawCall())
                ////}
                return _drawCalls;
            }
        }
        public int IsolineDivisionsU { get; set; } = 4;
        public int IsolineDivisionsV { get; set; } = 4;
        public bool ShowEdges { get; set; } = false;

        public event Action OnParametersChanged;

        public GregoryPatchGenerator(IBezierSurface a, IBezierSurface b, IBezierSurface c)
        {

        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public uint[] GetIndices()
        {
            throw new NotImplementedException();
        }

        public float[] GetVertices()
        {
            throw new NotImplementedException();
        }

        public void RefreshDataPreRender()
        {
            throw new NotImplementedException();
        }
    }
}
