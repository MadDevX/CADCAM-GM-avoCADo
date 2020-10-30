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
        public event Action OnSimulationFinished;
        public event Action OnCNCSimulatorUpdated;
        public bool SimulationFinished { get; private set; }

        public float SimulationSpeed { get; set; } = 0.01f;
        public bool Paused { get; set; } = true;

        public bool ShowPaths { get => _lineRenderer.Enabled; set => _lineRenderer.Enabled = value; }

        public int TextureWidth { get => _materialBlock.Width; set => _materialBlock.Width = value; }
        public int TextureHeight { get => _materialBlock.Height; set => _materialBlock.Height = value; }

        public float WorldWidth { get => _materialBlock.WorldWidth; set => _materialBlock.WorldWidth = value; }
        public float WorldHeight { get => _materialBlock.WorldHeight; set => _materialBlock.WorldHeight = value; }

        public float DefaultHeight { get => _materialBlock.DefaultHeightValue; set => _materialBlock.DefaultHeightValue = value; }
        public float BaseHeight { get => _materialBlock.MinHeightValue; set => _materialBlock.MinHeightValue = value; }

        private MaterialBlock _materialBlock;
        private readonly NodeFactory _nodeFactory;
        private LineRenderer _lineRenderer;
        private List<CNCInstructionSet> _instructionSets = new List<CNCInstructionSet>();
        public CNCSimulator Simulator { get; private set; }
        private int _currentInstSet;
        private INode _toolNode;

        private Vector3 _lastToolPos = Vector3.UnitY * 0.1f;

        public MillableSurface(MaterialBlock materialBlock, NodeFactory nodeFactory)
        {
            _materialBlock = materialBlock;
            _nodeFactory = nodeFactory;
        }

        public override void Initialize()
        {
            _lineRenderer = OwnerNode.GetComponent<LineRenderer>();
            base.Initialize();
        }

        public void SetPaths(List<CNCInstructionSet> instructionSets)
        {
            _instructionSets = instructionSets;
            ResetSimulationState();
        }

        public void ResetMaterial()
        {
            _materialBlock.ResetHeightMapValues();
        }

        public void ResetSimulationState()
        {
            Simulator = null;
            _currentInstSet = 0;
            SimulationFinished = false;
        }

        public void SkipSimulation()
        {
            try
            {
                while (Simulator != null)
                {
                    Simulator.AdvanceSimulation(float.MaxValue); //Simulator.InstructionSet.PathsLength * 10.0f if some numerical errors come out
                    _toolNode.Transform.Position = Simulator.CurrentToolPosition + new Vector3(0.0f, _instructionSets[_currentInstSet].Tool.Radius, 0.0f);
                    CleanupCNCSim();
                    UpdateCNCSimulator();
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

        protected override void OnUpdate(float deltaTime)
        {
            try
            {
                if (Paused == false)
                {
                    UpdateCNCSimulator();
                    if (Simulator != null)
                    {
                        var finished = Simulator.AdvanceSimulation(deltaTime * SimulationSpeed);
                        _toolNode.Transform.Position = Simulator.CurrentToolPosition + new Vector3(0.0f, _instructionSets[_currentInstSet].Tool.Radius, 0.0f);
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
            _lastToolPos = Simulator.CurrentToolPosition;
            Simulator = null;
            _currentInstSet++;
        }

        private void UpdateCNCSimulator()
        {
            if (Simulator == null)
            {
                if (_currentInstSet < _instructionSets.Count)
                {
                    Simulator = new CNCSimulator(_instructionSets[_currentInstSet], _materialBlock, _lastToolPos);
                    OnCNCSimulatorUpdated?.Invoke();
                    if (_toolNode != null)
                    {
                        _toolNode.Dispose();
                        _toolNode = null;
                    }
                    _toolNode = _nodeFactory.CreateTorus(0.0f, _instructionSets[_currentInstSet].Tool.Radius * 0.95f); 
                }
                else
                {
                    SimulationFinished = true;
                    OnSimulationFinished?.Invoke();
                }
            }
        }
    }
}
