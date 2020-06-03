using avoCADo.HUD;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo.Actions
{
    public class TransformationInstruction : Instruction<TransformationInstruction.Parameters>
    {
        private Parameters _parameters;
        
        public override bool Execute(Parameters parameters)
        {
            _parameters = parameters;
            return true;
        }

        public void Append(Vector3 diffVector)
        {
            throw new NotImplementedException();
        }

        public override bool Undo()
        {
            throw new NotImplementedException();
        }

        public struct Parameters
        {
            public TransformationMode transformationMode;
            public TransformationType transformationType;
        }
    }
}
