using avoCADo.CNC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace avoCADo.Components
{
    public class MillableSurface : UpdatableMComponent
    {
        public float SimulationSpeed { get; set; }

        public int TextureWidth { get => _materialBlock.Width; set => _materialBlock.Width = value; }
        public int TextureHeight { get => _materialBlock.Height; set => _materialBlock.Height = value; }

        public float WorldWidth { get => _materialBlock.WorldWidth; set => _materialBlock.WorldWidth = value; }
        public float WorldHeight { get => _materialBlock.WorldHeight; set => _materialBlock.WorldHeight = value; }

        public float DefaultHeight { get => _materialBlock.DefaultHeightValue; set => _materialBlock.DefaultHeightValue = value; }
        public float BaseHeight { get => _materialBlock.MinHeightValue; set => _materialBlock.MinHeightValue = value; }

        private MaterialBlock _materialBlock;

        private List<CNCInstructionSet> _instructionSets;

        public MillableSurface(MaterialBlock materialBlock)
        {
            _materialBlock = materialBlock;
        }

        public void SetPaths(List<CNCInstructionSet> instructionSets)
        {
            _instructionSets = instructionSets;
        }

        public void ResetMaterial()
        {
            _materialBlock.ResetHeightMapValues();
        }

        protected override void OnUpdate(float deltaTime)
        {
            //costamcostam
            
        }

        public void Simulate()
        {
            try
            {
                foreach (var set in _instructionSets)
                {
                    CNCSimulator.Execute(set, _materialBlock);
                }
            }
            catch (Exception e)
            {
                var message = e.InnerException != null ? e.InnerException.Message : e.Message;
                MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
