using avoCADo.Algebra;
using avoCADo.Architecture;
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
        private INode _createdNode;

        public override bool Execute(Parameters parameters)
        {
            ISurface p, q;

            if (parameters.a.Renderer.GetGenerator() is ISurfaceGenerator pGen && parameters.b.Renderer.GetGenerator() is ISurfaceGenerator qGen)
            {
                p = pGen.Surface;
                q = qGen.Surface;
            }
            else
            {
                MessageBox.Show("Provided nodes are not surfaces");
                return false;
            }

            var intersection = IntersectionFinder.FindIntersection(new IntersectionData(p, q), parameters.knotDistance);
            if (intersection == null)
            {
                MessageBox.Show("No intersection found");
                return false;
            }

            _createdNode = Registry.NodeFactory.CreateObject(ObjectType.IntersectionCurve, new IntersectionCurveParameters(p, q, intersection));
            return true;
        }

        public override bool Undo()
        {
            if (_createdNode != null)
            {
                _createdNode.Dispose();
                return true;
            }
            else return false;
        }

        public struct Parameters
        {
            public INode a;
            public INode b;
            public float knotDistance;
            public bool useStartingPoint;
            public Vector3 startingPoint;

            public Parameters(INode a, INode b, float knotDistance)
            {
                this.a = a;
                this.b = b;
                this.knotDistance = knotDistance;
                this.useStartingPoint = false;
                this.startingPoint = Vector3.Zero;
            }

            public Parameters(INode a, INode b, float knotDistance, Vector3 startingPoint) : this(a, b, knotDistance)
            {
                this.useStartingPoint = true;
                this.startingPoint = startingPoint;
            }
        }
    }
}
