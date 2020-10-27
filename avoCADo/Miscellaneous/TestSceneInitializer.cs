using avoCADo.Algebra;
using avoCADo.CNC;
using avoCADo.Components;
using avoCADo.Rendering.Renderers;
using avoCADo.Utility;
using Microsoft.Win32;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace avoCADo
{
    public static class TestSceneInitializer
    {
        public static void SpawnTestObjects(Scene scene, NodeFactory nodeFactory, IUpdateLoop loop, ShaderProvider provider, Cursor3D cursor3D)
        {
            //var point = nodeFactory.CreateObject(ObjectType.Point, null);
            //point.Transform.WorldPosition = Vector3.UnitX;
            //var point2 = nodeFactory.CreateObject(ObjectType.Point, null);
            //point2.Transform.WorldPosition = Vector3.UnitX * 2.0f;
            //var point3 = nodeFactory.CreateObject(ObjectType.Point, null);
            //point3.Transform.WorldPosition = Vector3.UnitY * 2.0f;
            //var point4 = nodeFactory.CreateObject(ObjectType.Point, null);
            //point4.Transform.WorldPosition = Vector3.UnitY * 3.0f;


            //var pointList = new List<INode> { point, point2, point3, point4 };
            //nodeFactory.CreateObject(ObjectType.InterpolatingCurve, new CurveParameters(pointList));
            ////nodeFactory.CreateObject(ObjectType.IntersectionCurve, new IntersectionCurveParameters());

            //nodeFactory.CreateObject(ObjectType.BezierPatchC0, new PatchParameters(WrapMode.Column));

            //var existingCP = new CoordList<INode>();
            //existingCP.ResetSize(4, 4);
            //for(int x = 0; x < 4; x++)
            //{
            //    for(int y = 0; y < 4; y++)
            //    {
            //        var node = nodeFactory.CreateObject(ObjectType.Point, null);
            //        node.Transform.WorldPosition = new Vector3(x, 0, y);
            //        existingCP[x, y] = node;
            //    }
            //}
            //nodeFactory.CreateObject(ObjectType.BezierPatchC2, new PatchParameters(WrapMode.None, 1, 1, 2.0f, 2.0f, existingCP));


            //cursor3D.Transform.WorldPosition = new Vector3(-6.0f, 0.0f, -2.0f);
            //nodeFactory.CreateObject(ObjectType.BezierPatchC0, new PatchParameters(WrapMode.None));
            //cursor3D.Transform.WorldPosition = new Vector3(-3.0f, 0.0f, -2.0f);
            //nodeFactory.CreateObject(ObjectType.BezierPatchC0, new PatchParameters(WrapMode.None));
            //cursor3D.Transform.WorldPosition = new Vector3(-4.5f, 0.0f, 0.0f);
            //nodeFactory.CreateObject(ObjectType.BezierPatchC0, new PatchParameters(WrapMode.None));
            //cursor3D.Transform.WorldPosition = new Vector3(0.0f, 0.0f, 0.0f);

            //var ret = LinearEquationSolver.Solve(Matrix4.Identity*2.0f, new Vector4(1.0f, 2.0f, 3.0f, 4.0f));
            //MessageBox.Show(ret.ToString());
            var instSetList = new List<CNCInstructionSet>();
            instSetList.Add(CNCInstructionParser.ParsePathFile("D:\\Studia\\Semestr I Mag\\MG1\\peukpaths\\1.k16"));
            instSetList.Add(CNCInstructionParser.ParsePathFile("D:\\Studia\\Semestr I Mag\\MG1\\peukpaths\\2.f12"));
            instSetList.Add(CNCInstructionParser.ParsePathFile("D:\\Studia\\Semestr I Mag\\MG1\\peukpaths\\3.f10"));
            instSetList.Add(CNCInstructionParser.ParsePathFile("D:\\Studia\\Semestr I Mag\\MG1\\peukpaths\\4.k08"));
            instSetList.Add(CNCInstructionParser.ParsePathFile("D:\\Studia\\Semestr I Mag\\MG1\\peukpaths\\5.k01"));


            var res = 300;
            var size = 0.18f;
            var mesh = MeshUtility.CreatePlaneMesh(res, res, size, size);
            var meshRenderer = new MeshRenderer(provider.MillableSurfaceShader, mesh, null);
            var block = new MaterialBlock(res, res, size, size, 0.2f, 0.0f, meshRenderer);
            var millableSurf = new MillableSurface(block);

            block.Width = 600;
            block.Height = 600;

            try
            {
                foreach (var set in instSetList)
                {
                    CNCSimulator.Execute(set, block);
                }
            }
            catch(Exception e)
            {
                var message = e.InnerException != null ? e.InnerException.Message : e.Message;
                MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            block.UpdateTexture();

            var node = new Node(new Transform(Vector3.Zero, Quaternion.Identity, Vector3.One), meshRenderer, "millableSurface");
            node.AttachComponents(millableSurf);
            scene.AttachChild(node);
        }

    }
}
