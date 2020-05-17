using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo.Utility
{
    /// <summary>
    /// Contains generic meshes to be used across all objects in application
    /// </summary>
    public static class MeshUtility
    {
        public static Mesh PointMesh { get; private set; }
        public static Mesh BoundMesh { get; set; } = null;

        public static void Initialize()
        {
            PointMesh = CreatePointMesh();
        }

        public static void Dispose()
        {
            PointMesh.Dispose();
            PointMesh = null;
        }

        private static Mesh CreatePointMesh()
        {
            var mesh = new Mesh();
            float[] vertices = { 0.0f, 0.0f, 0.0f };
            uint[] indices = { 0 };
            mesh.SetBufferData(vertices, indices, BufferUsageHint.StaticDraw);
            return mesh;
        }
    }
}
