using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo.Actions
{
    public class MergePointsInstruction : Instruction<MergePointsInstruction.Parameters>
    {
        public override bool Execute(Parameters parameters)
        {
            var depCollA = parameters.pointA as IDependencyCollector;
            var depCollB = parameters.pointB as IDependencyCollector;
            if (depCollB == null) throw new InvalidOperationException("Provided node does not implement IDependencyCollector interface");
            var depAddersOfPointB = new List<IDependencyAdder>();
            DependencyUtility.AddAllDependencies(depAddersOfPointB, parameters.pointB as IDependencyCollector);
            foreach(var depAdd in depAddersOfPointB)
            {
                depAdd.ReplaceDependency(current: depCollB, newDepColl: depCollA); //TODO: should return array of replaced indices (to enable reversing dependency replacement)
            }

            if (parameters.averagePosition)
            {
                var avgPosition = (parameters.pointA.Transform.WorldPosition + parameters.pointB.Transform.WorldPosition) * 0.5f;
                parameters.pointA.Transform.WorldPosition = avgPosition;
            }
            //TODO: gather execution data and reverse dependency replacement
            _instructionBuffer.Clear();
            return false;
        }

        public override bool Undo()
        {
            //TODO: gather execution data and reverse dependency replacement
            return true;
        }

        public struct Parameters
        {
            public INode pointA;
            public INode pointB;
            public bool averagePosition;

            public Parameters(INode pointA, INode pointB, bool averagePosition = true)
            {
                this.pointA = pointA;
                this.pointB = pointB;
                this.averagePosition = averagePosition;
            }
        }
    }
}
