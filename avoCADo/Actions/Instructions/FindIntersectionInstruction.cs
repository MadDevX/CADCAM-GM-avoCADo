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

            if (parameters.a.GetComponent<Renderer>()?.GetGenerator() is ISurfaceGenerator pGen && parameters.b.GetComponent<Renderer>()?.GetGenerator() is ISurfaceGenerator qGen)
            {
                p = pGen.Surface;
                q = qGen.Surface;
            }
            else
            {
                MessageBox.Show("Provided nodes are not surfaces");
                return false;
            }

            IList<Vector4> intersection = null;
            try
            {
                if (parameters.useStartingPoint)
                {
                    intersection = IntersectionFinder.FindIntersection(new IntersectionData(p, q), parameters.startingPoint, parameters.knotDistance);
                }
                else
                {
                    intersection = IntersectionFinder.FindIntersection(new IntersectionData(p, q), parameters.knotDistance);
                }
            }
            catch(Exception e)
            {
                MessageBox.Show($"Unexpected error occurred:\n + {e.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            if (intersection == null)
            {
                MessageBox.Show("No intersection found");
                return false;
            }

            _createdNode = Registry.NodeFactory.CreateObject(ObjectType.IntersectionCurve, new IntersectionCurveParameters(parameters.a, parameters.b, p, q, intersection));

            p.TrimTexture.UpdateTrimTexture(q, true, parameters.flipLoopsP);
            q.TrimTexture.UpdateTrimTexture(p, false, parameters.flipLoopsQ);
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
            public bool flipLoopsP;
            public bool flipLoopsQ;

            public Parameters(INode a, INode b, float knotDistance, bool flipLoopsP, bool flipLoopsQ)
            {
                this.a = a;
                this.b = b;
                this.knotDistance = knotDistance;
                this.useStartingPoint = false;
                this.startingPoint = Vector3.Zero;
                this.flipLoopsP = flipLoopsP;
                this.flipLoopsQ = flipLoopsQ;
            }

            public Parameters(INode a, INode b, float knotDistance, bool flipLoopsP, bool flipLoopsQ, Vector3 startingPoint) : this(a, b, knotDistance, flipLoopsP, flipLoopsQ)
            {
                this.useStartingPoint = true;
                this.startingPoint = startingPoint;
            }
        }
    }
}
