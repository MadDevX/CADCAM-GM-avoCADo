using OpenTK;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo
{
    public static class TestSceneInitializer
    {
        public static void SpawnTestObjects(Scene scene, Shader shader)
        {
            var parent = new Node(new Transform(new Vector3(1.0f, 0.0f, -1.0f), new Vector3(0.0f, MathHelper.DegreesToRadians(90.0f), 0.0f), Vector3.One), new MeshRenderer(shader, new TorusGenerator(30, 30, new TorusSurface(0.5f, 0.2f))), "parent torus");
            var child = new Node(new Transform(Vector3.UnitX, new Vector3(0.0f, 0.0f, MathHelper.DegreesToRadians(45.0f)), Vector3.One * 0.5f), new MeshRenderer(shader, new TorusGenerator(30, 30, new TorusSurface(0.5f, 0.2f))), "child torus");
            var point = new Node(new Transform(Vector3.UnitX, Vector3.Zero, Vector3.One), new PointRenderer(shader), "point");
            var point2 = new Node(new Transform(Vector3.UnitX * 2.0f, Vector3.Zero, Vector3.One), new PointRenderer(shader), "point2");
            var point3 = new Node(new Transform(Vector3.UnitY * 2.0f, Vector3.Zero, Vector3.One), new PointRenderer(shader), "point3");
            var point4 = new Node(new Transform(Vector3.UnitY * 3.0f, Vector3.Zero, Vector3.One), new PointRenderer(shader), "point4");
            parent.AttachChild(child);
            scene.AttachChild(parent);
            scene.AttachChild(new Node(new Transform(-Vector3.UnitX, new Vector3(0.0f, 0.0f, MathHelper.DegreesToRadians(45.0f)), Vector3.One * 0.5f), new MeshRenderer(shader, new TorusGenerator(30, 30, new TorusSurface(0.5f, 0.2f))), "child torus"));
            scene.AttachChild(new Node(new Transform(Vector3.UnitY, new Vector3(0.0f, 0.0f, MathHelper.DegreesToRadians(45.0f)), Vector3.One * 0.5f), new MeshRenderer(shader, new TorusGenerator(30, 30, new TorusSurface(0.5f, 0.2f))), "child torus"));
            scene.AttachChild(new Node(new Transform(-Vector3.UnitY, new Vector3(0.0f, 0.0f, MathHelper.DegreesToRadians(45.0f)), Vector3.One * 0.5f), new MeshRenderer(shader, new TorusGenerator(30, 30, new TorusSurface(0.5f, 0.2f))), "child torus"));
            scene.AttachChild(point);
            scene.AttachChild(point2);
            scene.AttachChild(point3);
            scene.AttachChild(point4);


            var bSplineSource = new ObservableCollection<INode>();
            var generator = new BSplineGenerator();
            var bSpline = new BSplineGroupNode(bSplineSource, new LineStripRenderer(shader, generator), generator, "bspline");
            bSpline.AttachChild(point);
            bSpline.AttachChild(point2);
            bSpline.AttachChild(point3);
            bSpline.AttachChild(point4);
            scene.AttachChild(bSpline);

            var bezierSource = new ObservableCollection<INode>();
            var curve = new BezierC0Curve(bezierSource);
            var bezGen = new BezierGeneratorNew(curve);
            var bezier = new BezierGroupNode(bezierSource, new LineStripRenderer(shader, bezGen), bezGen, "bezier");
            bezier.AttachChild(point);
            bezier.AttachChild(point2);
            bezier.AttachChild(point3);
            bezier.AttachChild(point4);
            scene.AttachChild(bezier);
        }

    }
}
