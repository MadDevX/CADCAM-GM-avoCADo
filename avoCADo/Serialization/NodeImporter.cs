using avoCADo.Serialization;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo
{
    public class NodeImporter
    {
        private NodeFactory _nodeFactory;

        public NodeImporter(NodeFactory nodeFactory)
        {
            _nodeFactory = nodeFactory;
        }


        public INode CreateNodeFromItem(NamedType item)
        {
            switch(item)
            {
                case SceneTorus torus:
                    var t = _nodeFactory.CreateTorus();
                    var genT = t.Renderer.GetGenerator() as TorusGenerator;
                    genT.XDivisions = int.Parse(torus.HorizontalSlices);
                    genT.YDivisions = int.Parse(torus.VerticalSlices);
                    var surf = genT.Surface as TorusSurface;
                    surf.MainRadius = (float)(torus.MajorRadius);
                    surf.TubeRadius = (float)(torus.MinorRadius);
                    t.Name = torus.Name;
                    t.Transform.WorldPosition = new OpenTK.Vector3((float)(torus.Position.X), (float)(torus.Position.Y), (float)(torus.Position.Z));
                    t.Transform.RotationEulerAngles = new OpenTK.Vector3((float)(torus.Rotation.X), (float)(torus.Rotation.Y), (float)(torus.Rotation.Z));
                    t.Transform.Scale = new OpenTK.Vector3((float)(torus.Scale.X), (float)(torus.Scale.Y), (float)(torus.Scale.Z));
                    return t;

                case ScenePoint point:
                    var p = _nodeFactory.CreatePoint();
                    p.Name = point.Name;
                    p.Transform.WorldPosition = new OpenTK.Vector3((float)(point.Position.X), (float)(point.Position.Y), (float)(point.Position.Z));
                    return p;

                case SceneBezierC0 bc0:
                    var nodeCC0 = _nodeFactory.CreateBezierCurveC0();
                    nodeCC0.Name = bc0.Name;
                    var genCC0 = nodeCC0.Renderer.GetGenerator() as BezierGeneratorGeometry;
                    genCC0.ShowEdges = bc0.ShowControlPolygon;
                    return nodeCC0;

                case SceneBezierC2 bc2:
                    var nodeCC2 = _nodeFactory.CreateBezierCurveC2();
                    nodeCC2.Name = bc2.Name;
                    var genCC2 = nodeCC2.Renderer.GetGenerator() as BezierGeneratorGeometry;
                    genCC2.ShowEdges = bc2.ShowBernsteinPolygon || bc2.ShowDeBoorPolygon; //TODO: divide de boor polygon representation
                    genCC2.ShowVirtualControlPoints = bc2.ShowBernsteinPoints;
                    return nodeCC2;

                case SceneBezierInter ic:
                    var nodeIC = _nodeFactory.CreateInterpolatingCurve();
                    nodeIC.Name = ic.Name;
                    var genIC = nodeIC.Renderer.GetGenerator() as BezierGeneratorGeometry;
                    genIC.ShowEdges = ic.ShowControlPolygon;
                    return nodeIC;

                //case ScenePatchC0 pc0:
                //    var type = pc0.WrapDirection != WrapType.None ? PatchType.Cylinder : PatchType.Flat;
                //    var nodePC0 = _nodeFactory.CreateBezierC0Patch(type, pc0.RowSlices/3???)
                default:
                    return null;
            }
        }
    }
}
