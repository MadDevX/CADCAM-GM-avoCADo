using avoCADo.Constants;
using avoCADo.Utility;
using OpenTK.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo.Components
{
    public class MillPathsManager : MComponent, IMeshGenerator
    {
        private MillableSurface _millableSurf;

        private IList<DrawCall> _drawCalls = new List<DrawCall>();
        public IList<DrawCall> DrawCalls
        {
            get
            {
                _drawCalls.Clear();
                if (_millableSurf.Simulator != null)
                {
                    var curInst = _millableSurf.Simulator.CurrentInstruction;
                    _drawCalls.Add(new DrawCall(0, 2 * Math.Max((curInst-1), 0), DrawCallShaderType.Default, RenderConstants.MILL_PATH_COMPLETED_SIZE, RenderConstants.MILL_PATH_COMPLETED_COLOR, RenderConstants.MILL_PATH_COMPLETED_COLOR));
                    _drawCalls.Add(new DrawCall(2 * Math.Max((curInst - 1), 0), 2, DrawCallShaderType.Default, RenderConstants.MILL_PATH_CURRENT_SIZE, RenderConstants.MILL_PATH_CURRENT_COLOR, RenderConstants.MILL_PATH_CURRENT_COLOR));
                    _drawCalls.Add(new DrawCall(2 * curInst, _indices.Length, DrawCallShaderType.Default, RenderConstants.MILL_PATH_SIZE, RenderConstants.MILL_PATH_COLOR, RenderConstants.MILL_PATH_COLOR));
                }
                return _drawCalls;
            }
        }

        public event Action OnParametersChanged;

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

        public override void Initialize()
        {
            _millableSurf = OwnerNode.GetComponent<MillableSurface>();
            _millableSurf.OnCNCSimulatorUpdated += SetupPathsToRender;
            base.Initialize();

        }

        public override void Dispose()
        {
            _millableSurf.OnCNCSimulatorUpdated -= SetupPathsToRender;
            base.Dispose();
        }

        public void RefreshDataPreRender()
        {
        }

        private void SetupPathsToRender()
        {
            var insts = _millableSurf.Simulator.InstructionSet.Instructions;
            Array.Resize(ref _vertices, insts.Count * 3);
            Array.Resize(ref _indices, 2 * (insts.Count - 1));
            for (uint i = 0; i < _indices.Length; i++) _indices[i] = (i+1) / 2;
            for(int i = 0; i < insts.Count; i++)
            {
                VBOUtility.SetVertex(_vertices, insts[i].Position, i);
            }
            OnParametersChanged?.Invoke();
        }
    }
}
