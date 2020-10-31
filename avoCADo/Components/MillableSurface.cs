using avoCADo.CNC;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace avoCADo.Components
{
    public class MillableSurface : UpdatableMComponent
    {
        public event Action OnSimulationFinished;
        public event Action OnCNCSimulatorUpdated;
        public event Action UpdateProgress;
        public bool SimulationInProgress { get; private set; } = false;

        public float SimulationSpeed { get; set; } = 0.01f;
        public float ToolHeight { get; set; } = 0.04f;
        public bool OverrideToolSettings { get; set; }
        private float _overrideToolRadius;
        public float OverrideToolRadius { get => _overrideToolRadius; set => _overrideToolRadius = Math.Max(value, 0.0f); }
        public CNCToolType OverrideToolType { get; set; } = CNCToolType.Flat;

        public bool Paused { get; set; } = false;

        public bool ShowPaths { get => _lineRenderer.Enabled; set => _lineRenderer.Enabled = value; }
        public int UseTexture { get => _materialBlock.UseTexture; set => _materialBlock.UseTexture = value; }

        public int TextureWidth { get => _materialBlock.Width; set => _materialBlock.Width = value; }
        public int TextureHeight { get => _materialBlock.Height; set => _materialBlock.Height = value; }

        public float WorldWidth { get => _materialBlock.WorldWidth; set => _materialBlock.WorldWidth = value; }
        public float WorldHeight { get => _materialBlock.WorldHeight; set => _materialBlock.WorldHeight = value; }

        public float DefaultHeight { get => _materialBlock.DefaultHeightValue; set => _materialBlock.DefaultHeightValue = value; }
        public float BaseHeight { get => _materialBlock.MinHeightValue; set => _materialBlock.MinHeightValue = value; }

        public float SimulationProgress
        {
            get
            {
                if(Simulator != null)
                {
                    return Simulator.CurrentInstruction / (float)(Simulator.InstructionCount - 1) * 100.0f;
                }
                else
                {
                    return 0.0f;
                }
            }
        }



        private MaterialBlock _materialBlock;
        private readonly NodeFactory _nodeFactory;
        private LineRenderer _lineRenderer;
        private List<CNCInstructionSet> _instructionSets = new List<CNCInstructionSet>();
        public CNCSimulator Simulator { get; private set; }
        private int _currentInstSet;
        private INode _toolNode;

        private bool _isSkipping = false;
        public bool IsSkipping 
        {
            get => _isSkipping;
            private set
            {
                _isSkipping = value;
                _materialBlock.OmitTextureUpdate = value;
            }
        }

        public IList<string> InstructionSetNames => _instructionSets.Select((x) => x.Name).ToList();
        public int InstructionSetCount => _instructionSets.Count;

        private Vector3 _lastToolPos = Vector3.UnitY * 0.1f;

        private static float _toolRadiusMult = 0.95f; //to minimize z-fighting and sinking of tool mesh into the material

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
            if(OverrideToolSettings)
            {
                var overrideTool = new CNCTool(OverrideToolType, OverrideToolRadius, ToolHeight);
                foreach(var instSet in _instructionSets)
                {
                    instSet.Tool = overrideTool;
                }
            }
            ResetSimulationState();
        }

        public void ResetMaterial()
        {
            _materialBlock.ResetHeightMapValues();
        }

        private void ResetSimulationState()
        {
            Simulator = null;
            _currentInstSet = 0;
            IsSkipping = false;
        }

        public void StartSimulation()
        {
            ResetSimulationState();
            SimulationInProgress = true;
            Paused = false;
        }

        public void SkipSimulation()
        {
            if (IsSkipping == false)
            {
                IsSkipping = true;
                DisposeToolGizmo();
                var thread = new Thread(
                    () =>
                        {
                            try
                            {
                                if (Simulator == null && SimulationInProgress)
                                {
                                    UpdateCNCSimulator(false);
                                }
                                while (Simulator != null)
                                {
                                    Simulator.AdvanceSimulation(float.MaxValue); //Simulator.InstructionSet.PathsLength * 10.0f if some numerical errors come out
                                    CleanupCNCSim();
                                    UpdateCNCSimulator(false);
                                }
                            }
                            catch (Exception e)
                            {
                                var message = e.InnerException != null ? e.InnerException.Message : e.Message;
                                CleanupCNCSim();
                                _instructionSets.Clear();
                                MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                ResetSimulationState();
                            }
                        }
                    );
                thread.Start();
            }
        }

        private void UpdateToolPosition()
        {
            if (_toolNode != null)
            {
                var tool = _instructionSets[_currentInstSet].Tool;
                if (tool.Type == CNCToolType.Round)
                {
                    _toolNode.Transform.Position = Simulator.CurrentToolPosition + new Vector3(0.0f, tool.Radius, 0.0f);
                }
                else
                {
                    _toolNode.Transform.Position = Simulator.CurrentToolPosition;
                }
            }
        }

        protected override void OnUpdate(float deltaTime)
        {
            try
            {
                if(IsSkipping)
                {
                    UpdateProgress?.Invoke();
                    if (SimulationInProgress == false)
                    {
                        IsSkipping = false;
                        OnSimulationFinished?.Invoke();
                    }
                    
                }
                else if (Paused == false && SimulationInProgress)
                {
                    UpdateProgress?.Invoke();
                    UpdateCNCSimulator();
                    if (Simulator != null)
                    {
                        var finished = Simulator.AdvanceSimulation(deltaTime * SimulationSpeed);
                        UpdateToolPosition();
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
                ResetSimulationState();
            }
        }

        private void CleanupCNCSim()
        {
            _lastToolPos = Simulator.CurrentToolPosition;
            Simulator = null;
            _currentInstSet++;
            DisposeToolGizmo();
        }

        private void DisposeToolGizmo()
        {
            if (_toolNode != null)
            {
                _toolNode.Dispose();
                _toolNode = null;
            }
        }

        private void UpdateCNCSimulator(bool realTimeSimulation = true)
        {
            if (Simulator == null)
            {
                if (_currentInstSet < _instructionSets.Count)
                {
                    Simulator = new CNCSimulator(_instructionSets[_currentInstSet], _materialBlock, _lastToolPos);
                    if (realTimeSimulation)
                    {
                        CreateToolGizmo();
                        OnCNCSimulatorUpdated?.Invoke();
                    }
                }
                else
                {
                    SimulationInProgress = false;
                    if (realTimeSimulation)
                    {
                        OnSimulationFinished?.Invoke();
                    }
                }
            }
        }

        private void CreateToolGizmo()
        {
            if (_toolNode != null)
            {
                throw new InvalidOperationException("Previous tool gizmo was not properly disposed!");
            }
            var tool = _instructionSets[_currentInstSet].Tool;
            if (tool.Type == CNCToolType.Round)
            {
                _toolNode = _nodeFactory.CreateTorus(OwnerNode, 0.0f, tool.Radius * _toolRadiusMult);
                var cyl = _nodeFactory.CreateCylinder(_toolNode, tool.Radius * _toolRadiusMult, tool.Height);
                cyl.Transform.Position = Vector3.Zero;
            }
            else
            {
                _toolNode = _nodeFactory.CreateCylinder(OwnerNode, tool.Radius * _toolRadiusMult, tool.Height);
            }
        }
    }
}
