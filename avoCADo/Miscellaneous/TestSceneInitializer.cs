using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo
{
    public static class TestSceneInitializer
    {
        public static void SpawnTestObjects(Scene scene, Shader shader)
        {
            var parent = new Node(new Transform(new Vector3(1.0f, 0.0f, -1.0f), new Vector3(0.0f, MathHelper.DegreesToRadians(90.0f), 0.0f), Vector3.One), new MeshRenderer(shader, new TorusGenerator(0.5f, 0.2f, 30, 30)), "parent torus");
            var child = new Node(new Transform(Vector3.UnitX, new Vector3(0.0f, 0.0f, MathHelper.DegreesToRadians(45.0f)), Vector3.One * 0.5f), new MeshRenderer(shader, new TorusGenerator(0.5f, 0.2f, 30, 30)), "child torus");
            var point = new Node(new Transform(Vector3.UnitX, Vector3.Zero, Vector3.One), new PointRenderer(shader), "point");
            parent.AttachChild(child);
            scene.AttachChild(parent);
            scene.AttachChild(new Node(new Transform(-Vector3.UnitX, new Vector3(0.0f, 0.0f, MathHelper.DegreesToRadians(45.0f)), Vector3.One * 0.5f), new MeshRenderer(shader, new TorusGenerator(0.5f, 0.2f, 20, 20)), "child torus"));
            scene.AttachChild(new Node(new Transform(Vector3.UnitY, new Vector3(0.0f, 0.0f, MathHelper.DegreesToRadians(45.0f)), Vector3.One * 0.5f), new MeshRenderer(shader, new TorusGenerator(0.5f, 0.2f, 10, 10)), "child torus"));
            scene.AttachChild(new Node(new Transform(-Vector3.UnitY, new Vector3(0.0f, 0.0f, MathHelper.DegreesToRadians(45.0f)), Vector3.One * 0.5f), new MeshRenderer(shader, new TorusGenerator(0.5f, 0.2f, 5, 5)), "child torus"));
            scene.AttachChild(point);
        }

    }
}
