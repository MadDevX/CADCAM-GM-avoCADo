using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo.Architecture
{
    public static class Registry
    {
        private static ICamera _camera = null;
        public static ICamera Camera
        {
            get => _camera;
            set
            {
                if (_camera != null) return;
                _camera = value;
            }
        }


        private static IUpdateLoop _updateLoop = null;
        public static IUpdateLoop UpdateLoop
        {
            get => _updateLoop;
            set
            {
                if (_updateLoop != null) return;
                _updateLoop = value;
            }
        }

        private static VirtualNodeFactory _virtualNodeFactory = null;
        public static VirtualNodeFactory VirtualNodeFactory 
        {
            get => _virtualNodeFactory;
            set
            {
                if (_virtualNodeFactory != null) return;
                _virtualNodeFactory = value;
            }
        }

        private static IInstructionBuffer _instructionBuffer = null;
        public static IInstructionBuffer InstructionBuffer
        {
            get => _instructionBuffer;
            set
            {
                if (_instructionBuffer != null) return;
                _instructionBuffer = value;
            }
        }

        private static NodeFactory _nodeFactory = null;
        public static NodeFactory NodeFactory
        {
            get => _nodeFactory;
            set
            {
                if (_nodeFactory != null) return;
                _nodeFactory = value;
            }
        }

        private static ShaderProvider _shaderProvider;
        public static ShaderProvider ShaderProvider
        {
            get => _shaderProvider;
            set
            {
                if (_shaderProvider != null) return;
                _shaderProvider = value;
            }
        }
    }
}
