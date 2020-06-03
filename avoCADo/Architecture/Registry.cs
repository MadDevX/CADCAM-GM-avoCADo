﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo.Architecture
{
    public static class Registry
    {
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
    }
}
