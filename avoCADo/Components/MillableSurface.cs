using avoCADo.CNC;
using OpenTK;
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
        public float SimulationSpeed { get; set; } = 0.01f;
        public bool Paused { get; set; } = true;

        public int TextureWidth { get => _materialBlock.Width; set => _materialBlock.Width = value; }
        public int TextureHeight { get => _materialBlock.Height; set => _materialBlock.Height = value; }

        public float WorldWidth { get => _materialBlock.WorldWidth; set => _materialBlock.WorldWidth = value; }
        public float WorldHeight { get => _materialBlock.WorldHeight; set => _materialBlock.WorldHeight = value; }

        public float DefaultHeight { get => _materialBlock.DefaultHeightValue; set => _materialBlock.DefaultHeightValue = value; }
        public float BaseHeight { get => _materialBlock.MinHeightValue; set => _materialBlock.MinHeightValue = value; }

        private MaterialBlock _materialBlock;
        private readonly NodeFactory _nodeFactory;
        private List<CNCInstructionSet> _instructionSets = new List<CNCInstructionSet>();
        private CNCSimulator _simulator;
        private int _currentInstSet;
        private INode _toolNode;

        private Vector3 _lastToolPos = Vector3.UnitY * 0.1f;

        public MillableSurface(MaterialBlock materialBlock, NodeFactory nodeFactory)
        {
            _materialBlock = materialBlock;
            _nodeFactory = nodeFactory;
        }

        public void SetPaths(List<CNCInstructionSet> instructionSets)
        {
            _instructionSets = instructionSets;
            _currentInstSet = 0;
            _simulator = null;
        }

        public void ResetMaterial()
        {
            _materialBlock.ResetHeightMapValues();
        }

        public void SkipSim()
        {
            while(_simulator != null)
            {
                _simulator.AdvanceSimulation(_simulator.DistanceToEnd);
                _toolNode.Transform.Position = _simulator.CurrentToolPosition + new Vector3(0.0f, _instructionSets[_currentInstSet].Tool.Radius, 0.0f);
                CleanupCNCSim();
                UpdateCNCSimulator();
            }
        }

        protected override void OnUpdate(float deltaTime)
        {
            try
            {
                if (Paused == false)
                {
                    UpdateCNCSimulator();
                    if (_simulator != null)
                    {
                        var finished = _simulator.AdvanceSimulation(deltaTime * SimulationSpeed);
                        _toolNode.Transform.Position = _simulator.CurrentToolPosition + new Vector3(0.0f, _instructionSets[_currentInstSet].Tool.Radius, 0.0f);
                        if (finished)
                        {
                            CleanupCNCSim();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                var message = e.InnerException != null ? e.InnerException.Message : e.Message;
                CleanupCNCSim();
                _instructionSets.Clear();
                MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CleanupCNCSim()
        {
            _lastToolPos = _simulator.CurrentToolPosition;
            _simulator = null;
            _currentInstSet++;
        }

        private void UpdateCNCSimulator()
        {
            if (_simulator == null && _currentInstSet < _instructionSets.Count)
            {
                _simulator = new CNCSimulator(_instructionSets[_currentInstSet], _materialBlock, _lastToolPos);
                if (_toolNode != null)
                {
                    _toolNode.Dispose();
                    _toolNode = null;
                }
                _toolNode = _nodeFactory.CreateTorus(0.0f, _instructionSets[_currentInstSet].Tool.Radius * 0.95f);
            }
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
