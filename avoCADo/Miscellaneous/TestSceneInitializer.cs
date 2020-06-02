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
        public static void SpawnTestObjects(Scene scene, NodeFactory nodeFactory, IUpdateLoop loop, ShaderProvider provider)
        {
            //var parent = new Node(new Transform(new Vector3(1.0f, 0.0f, -1.0f), new Vector3(0.0f, MathHelper.DegreesToRadians(90.0f), 0.0f), Vector3.One), new MeshRenderer(shader, new TorusGenerator(30, 30, new TorusSurface(0.5f, 0.2f))), "parent torus");
            //var child = new Node(new Transform(Vector3.UnitX, new Vector3(0.0f, 0.0f, MathHelper.DegreesToRadians(45.0f)), Vector3.One * 0.5f), new MeshRenderer(shader, new TorusGenerator(30, 30, new TorusSurface(0.5f, 0.2f))), "child torus");
            var point = new Node(new Transform(Vector3.UnitX, Vector3.Zero, Vector3.One),         new PointRenderer(provider.DefaultShader, Color4.Orange, Color4.Yellow), "point");
            var point2 = new Node(new Transform(Vector3.UnitX * 2.0f, Vector3.Zero, Vector3.One), new PointRenderer(provider.DefaultShader, Color4.Orange, Color4.Yellow), "point2");
            var point3 = new Node(new Transform(Vector3.UnitY * 2.0f, Vector3.Zero, Vector3.One), new PointRenderer(provider.DefaultShader, Color4.Orange, Color4.Yellow), "point3");
            var point4 = new Node(new Transform(Vector3.UnitY * 3.0f, Vector3.Zero, Vector3.One), new PointRenderer(provider.DefaultShader, Color4.Orange, Color4.Yellow), "point4");
            //parent.AttachChild(child);
            //scene.AttachChild(parent);
            //scene.AttachChild(new Node(new Transform(-Vector3.UnitX, new Vector3(0.0f, 0.0f, MathHelper.DegreesToRadians(45.0f)), Vector3.One * 0.5f), new MeshRenderer(shader, new TorusGenerator(30, 30, new TorusSurface(0.5f, 0.2f))), "child torus"));
            //scene.AttachChild(new Node(new Transform(Vector3.UnitY, new Vector3(0.0f, 0.0f, MathHelper.DegreesToRadians(45.0f)), Vector3.One * 0.5f), new MeshRenderer(shader, new TorusGenerator(30, 30, new TorusSurface(0.5f, 0.2f))), "child torus"));
            //scene.AttachChild(new Node(new Transform(-Vector3.UnitY, new Vector3(0.0f, 0.0f, MathHelper.DegreesToRadians(45.0f)), Vector3.One * 0.5f), new MeshRenderer(shader, new TorusGenerator(30, 30, new TorusSurface(0.5f, 0.2f))), "child torus"));
            scene.AttachChild(point);
            scene.AttachChild(point2);
            scene.AttachChild(point3);
            scene.AttachChild(point4);

            var bezierSource = new WpfObservableRangeCollection<INode>();
            var curve =  new InterpolatingC2Curve(bezierSource);
            var bezGen = new BezierGeneratorGeometry(curve);
            var bezier = new BezierGeomGroupNode(bezierSource, new ParametricObjectRenderer(provider.SurfaceShaderBezier, provider.SurfaceShaderDeBoor, provider.CurveShader, provider.DefaultShader, bezGen), bezGen, "interpolatingCurve");
            //var bezGen = new BezierGeneratorNew(curve); //new BezierGeneratorNew(curve);
            //var bezier = new BezierGroupNode(bezierSource, new LineRenderer(shader, bezGen), bezGen, "bezier");
            bezier.AttachChild(point);
            bezier.AttachChild(point2);
            bezier.AttachChild(point3);
            bezier.AttachChild(point4);
            scene.AttachChild(bezier);

            var bezierSurfCollection = new WpfObservableRangeCollection<INode>();
            var surface = new BezierC0Patch();
            var surfGen = new BezierPatchGenerator(surface, nodeFactory, PatchType.Cylinder, Vector3.Zero);
            var surfNode = new BezierPatchGroupNode(bezierSurfCollection, new ParametricObjectRenderer(provider.SurfaceShaderBezier, provider.SurfaceShaderDeBoor, provider.CurveShader, provider.DefaultShader, surfGen), surfGen, "BezierPatch");
            scene.AttachChild(surfNode);
            //surfGen.SurfaceWidthOrRadius = 2;
            //surfGen.SurfaceHeight = 2;
            //surfGen.HorizontalPatches = 2;
            //surfGen.VerticalPatches = 2;
            nodeFactory.CreateBezierPatchC2(new PatchParameters(PatchType.Cylinder, 3, 3, 1.0f, 1.0f));
        }

    }
}
