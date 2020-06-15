using OpenTK;
using OpenTK.Graphics;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace avoCADo
{
    public static class TestSceneInitializer
    {
        public static void SpawnTestObjects(Scene scene, NodeFactory nodeFactory, IUpdateLoop loop, ShaderProvider provider, Cursor3D cursor3D)
        {
            var point = nodeFactory.CreateObject(ObjectType.Point, null);
            point.Transform.WorldPosition = Vector3.UnitX;
            var point2 = nodeFactory.CreateObject(ObjectType.Point, null);
            point2.Transform.WorldPosition = Vector3.UnitX * 2.0f;
            var point3 = nodeFactory.CreateObject(ObjectType.Point, null);
            point3.Transform.WorldPosition = Vector3.UnitY * 2.0f;
            var point4 = nodeFactory.CreateObject(ObjectType.Point, null);
            point4.Transform.WorldPosition = Vector3.UnitY * 3.0f;


            nodeFactory.CreateObject(ObjectType.InterpolatingCurve, new CurveParameters(new List<INode> { point, point2, point3, point4 }));

            nodeFactory.CreateObject(ObjectType.BezierPatchC0, new PatchParameters(WrapMode.Column));

            var existingCP = new CoordList<INode>();
            existingCP.ResetSize(4, 4);
            for(int x = 0; x < 4; x++)
            {
                for(int y = 0; y < 4; y++)
                {
                    var node = nodeFactory.CreateObject(ObjectType.Point, null);
                    node.Transform.WorldPosition = new Vector3(x, 0, y);
                    existingCP[x, y] = node;
                }
            }
            nodeFactory.CreateObject(ObjectType.BezierPatchC2, new PatchParameters(WrapMode.None, 1, 1, 2.0f, 2.0f, existingCP));


            cursor3D.Transform.WorldPosition = new Vector3(-6.0f, 0.0f, -2.0f);
            nodeFactory.CreateObject(ObjectType.BezierPatchC0, new PatchParameters(WrapMode.None));
            cursor3D.Transform.WorldPosition = new Vector3(-3.0f, 0.0f, -2.0f);
            nodeFactory.CreateObject(ObjectType.BezierPatchC0, new PatchParameters(WrapMode.None));
            cursor3D.Transform.WorldPosition = new Vector3(-4.5f, 0.0f, 0.0f);
            nodeFactory.CreateObject(ObjectType.BezierPatchC0, new PatchParameters(WrapMode.None));
            cursor3D.Transform.WorldPosition = new Vector3(0.0f, 0.0f, 0.0f);
        }

    }
}
