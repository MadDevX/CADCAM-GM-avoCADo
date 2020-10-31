﻿using avoCADo.CNC;
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
        public bool SimulationInProgress { get; private set; } = false;

        public float SimulationSpeed { get; set; } = 0.01f;
        public bool Paused { get; set; } = false;

        public bool ShowPaths { get => _lineRenderer.Enabled; set => _lineRenderer.Enabled = value; }
        public int UseTexture { get => _materialBlock.UseTexture; set => _materialBlock.UseTexture = value; }

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
        }

        public void StartSimulation()
        {
            ResetSimulationState();
            SimulationInProgress = true;
            Paused = false;
        }

        public void SkipSimulation()
        {
            try
            {
                if(Simulator == null && SimulationInProgress)
                {
                    UpdateCNCSimulator();
                }
                while (Simulator != null)
                {
                    Simulator.AdvanceSimulation(float.MaxValue); //Simulator.InstructionSet.PathsLength * 10.0f if some numerical errors come out
                    UpdateToolPosition();
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
                if (Paused == false && SimulationInProgress)
                {
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
                    var tool = _instructionSets[_currentInstSet].Tool;
                    if (tool.Type == CNCToolType.Round)
                    {
                        _toolNode = _nodeFactory.CreateTorus(0.0f, tool.Radius * _toolRadiusMult);
                        var cyl = _nodeFactory.CreateCylinder(_toolNode, tool.Radius * _toolRadiusMult, tool.Height);
                        cyl.Transform.Position = Vector3.Zero;
                    }
                    else
                    {
                        _toolNode = _nodeFactory.CreateCylinder(OwnerNode, tool.Radius * _toolRadiusMult, tool.Height);
                    }
                }
                else
                {
                    SimulationInProgress = false;
                    OnSimulationFinished?.Invoke();
                }
            }
        }
    }
}
