using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace avoCADo.Actions
{
    using Parameters = FindIntersectionInstruction.Parameters;
    public class FindIntersectionInstruction : Instruction<Parameters>
    {
        public override bool Execute(Parameters parameters)
        {
            MessageBox.Show("Find intersection executed");
            return true;
        }

        public override bool Undo()
        {
            MessageBox.Show("Find intersection undo");
            return true;
        }

        public struct Parameters
        {
            public INode a;
            public INode b;
            public float step;
            public bool useStartingPoint;
            public Vector3 startingPoint;

            public Parameters(INode a, INode b, float step)
            {
                this.a = a;
                this.b = b;
                this.step = step;
                this.useStartingPoint = false;
                this.startingPoint = Vector3.Zero;
            }

            public Parameters(INode a, INode b, float step, Vector3 startingPoint) : this(a, b, step)
            {
                this.useStartingPoint = true;
                this.startingPoint = startingPoint;
            }
        }
    }
}
