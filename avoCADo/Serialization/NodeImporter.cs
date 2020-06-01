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
                    var gen = t.Renderer.GetGenerator() as TorusGenerator;
                    gen.XDivisions = int.Parse(torus.HorizontalSlices);
                    gen.YDivisions = int.Parse(torus.VerticalSlices);
                    var surf = gen.Surface as TorusSurface;
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

                default:
                    return null;
            }
        }
    }
}
