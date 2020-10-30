using OpenTK;
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

        /// <summary>
        /// Represents currently bound mesh.
        /// </summary>
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
            var mesh = new Mesh(VertexLayout.Type.Position);
            float[] vertices = { 0.0f, 0.0f, 0.0f };
            uint[] indices = { 0 };
            mesh.SetBufferData(vertices, indices, BufferUsageHint.StaticDraw);
            return mesh;
        }

        public static Mesh CreatePlaneMesh(int xVertices, int yVertices, float width, float height)
        {
            var layout = VertexLayout.Type.PositionTexCoord;
            var mesh = new Mesh(layout);
            var vertexCount = xVertices * yVertices;
            var vertices = new float[VertexLayout.Stride(layout) * vertexCount];
            var indices = new uint[6 * (xVertices-1) * (yVertices-1)];

            int curIdx = 0;
            for(int y = 0; y < yVertices; y++)
            {
                for(int x = 0; x < xVertices; x++)
                {
                    var u = ((float)x) / (xVertices - 1);
                    var v = ((float)y) / (yVertices - 1);
                    var pos = new Vector3(-width * 0.5f + u * width, 0.0f, -height * 0.5f + v * height);
                    VBOUtility.SetVertex(vertices, pos, new Vector2(u, v), curIdx);
                    curIdx++;
                }
            }

            curIdx = 0;
            for(uint y = 0; y < (yVertices-1) ; y++)
            {
                for (uint x = 0; x < (xVertices-1); x++)
                {
                    indices[curIdx++] = (uint)(x + y * xVertices);
                    indices[curIdx++] = (uint)(x + (y+1) * xVertices);
                    indices[curIdx++] = (uint)((x+1) + y * xVertices);
                    indices[curIdx++] = (uint)(x + (y+1) * xVertices);
                    indices[curIdx++] = (uint)((x+1) + (y+1) * xVertices);
                    indices[curIdx++] = (uint)((x+1) + y * xVertices);
                }
            }

            mesh.SetBufferData(vertices, indices, BufferUsageHint.DynamicDraw);

            return mesh;
        }

        public static Mesh CreateMillableMesh(int xVertices, int yVertices, float width, float height)
        {
            var layout = VertexLayout.Type.PositionNormalTexCoord;
            var mesh = new Mesh(layout);
            var vertexCount = xVertices * yVertices;
            //                                                           *2 because of base         mesh sides
            var vertices = new float[VertexLayout.Stride(layout) * (vertexCount * 2 +       (xVertices+yVertices) * 4)];
            //                              top                           base                sides
            var indices = new uint[6 * (xVertices - 1) * (yVertices - 1) * 2 + ((xVertices-1) + (yVertices-1)) * 2 * 6];

            var baseOffset = xVertices * yVertices;
            int curIdx = 0;
            for (int y = 0; y < yVertices; y++)
            {
                for (int x = 0; x < xVertices; x++)
                {
                    var u = ((float)x) / (xVertices - 1);
                    var v = ((float)y) / (yVertices - 1);
                    var pos = new Vector3(-width * 0.5f + u * width, 0.0f, -height * 0.5f + v * height);
                    var posTop = pos;
                    posTop.Y = float.NaN; //to detect in shader (this will also work for sides, which have normal in a different direction than Up
                    VBOUtility.SetVertex(vertices, posTop, Vector3.UnitY, new Vector2(u, v), curIdx); //top
                    VBOUtility.SetVertex(vertices, pos, -Vector3.UnitY, new Vector2(u, v), curIdx + baseOffset); //bottom
                    curIdx++;
                }
            }
            curIdx = 2 * baseOffset;
            #region Sides
            for (int x = 0; x < xVertices; x++)
            {
                var u = ((float)x) / (xVertices - 1);
                var v = 0.0f;
                var pos = new Vector3(-width * 0.5f + u * width, 0.0f, -height * 0.5f + v * height);
                var posTop = pos;
                posTop.Y = float.NaN; //to detect in shader (this will also work for sides, which have normal in a different direction than Up
                VBOUtility.SetVertex(vertices, posTop, -Vector3.UnitZ, new Vector2(u, v), curIdx++); //top
                VBOUtility.SetVertex(vertices, pos, -Vector3.UnitZ, new Vector2(u, v), curIdx++); //bottom
            }
            for (int x = 0; x < xVertices; x++)
            {
                var u = ((float)x) / (xVertices - 1);
                var v = 1.0f;
                var pos = new Vector3(-width * 0.5f + u * width, 0.0f, -height * 0.5f + v * height);
                var posTop = pos;
                posTop.Y = float.NaN; //to detect in shader (this will also work for sides, which have normal in a different direction than Up
                VBOUtility.SetVertex(vertices, posTop, Vector3.UnitZ, new Vector2(u, v), curIdx++); //top
                VBOUtility.SetVertex(vertices, pos, Vector3.UnitZ, new Vector2(u, v), curIdx++); //bottom
            }
            for (int y = 0; y < yVertices; y++)
            {
                var u = 0.0f;
                var v = ((float)y) / (yVertices - 1);
                var pos = new Vector3(-width * 0.5f + u * width, 0.0f, -height * 0.5f + v * height);
                var posTop = pos;
                posTop.Y = float.NaN; //to detect in shader (this will also work for sides, which have normal in a different direction than Up
                VBOUtility.SetVertex(vertices, posTop, -Vector3.UnitX, new Vector2(u, v), curIdx++); //top
                VBOUtility.SetVertex(vertices, pos, -Vector3.UnitX, new Vector2(u, v), curIdx++); //bottom
            }
            for (int y = 0; y < yVertices; y++)
            {
                var u = 1.0f;
                var v = ((float)y) / (yVertices - 1);
                var pos = new Vector3(-width * 0.5f + u * width, 0.0f, -height * 0.5f + v * height);
                var posTop = pos;
                posTop.Y = float.NaN; //to detect in shader (this will also work for sides, which have normal in a different direction than Up
                VBOUtility.SetVertex(vertices, posTop, Vector3.UnitX, new Vector2(u, v), curIdx++); //top
                VBOUtility.SetVertex(vertices, pos, Vector3.UnitX, new Vector2(u, v), curIdx++); //bottom
            }
            #endregion

            curIdx = 0;
            for (uint y = 0; y < (yVertices - 1); y++)
            {
                for (uint x = 0; x < (xVertices - 1); x++)
                {
                    indices[curIdx++] = (uint)(x + y * xVertices);
                    indices[curIdx++] = (uint)(x + (y + 1) * xVertices);
                    indices[curIdx++] = (uint)((x + 1) + y * xVertices);
                    indices[curIdx++] = (uint)(x + (y + 1) * xVertices);
                    indices[curIdx++] = (uint)((x + 1) + (y + 1) * xVertices);
                    indices[curIdx++] = (uint)((x + 1) + y * xVertices);
                }
            }
            for (uint y = 0; y < (yVertices - 1); y++)
            {
                for (uint x = 0; x < (xVertices - 1); x++)
                {
                    indices[curIdx++] = (uint)(x + y * xVertices             + baseOffset);
                    indices[curIdx++] = (uint)(x + (y + 1) * xVertices       + baseOffset);
                    indices[curIdx++] = (uint)((x + 1) + y * xVertices       + baseOffset);
                    indices[curIdx++] = (uint)(x + (y + 1) * xVertices       + baseOffset);
                    indices[curIdx++] = (uint)((x + 1) + (y + 1) * xVertices + baseOffset);
                    indices[curIdx++] = (uint)((x + 1) + y * xVertices       + baseOffset);
                }
            }

            uint xSidesOffset = 2 * (uint)baseOffset;
            for (uint x = xSidesOffset; x < (xVertices - 1) * 2 + xSidesOffset; x+=2)
            {
                indices[curIdx++] = (uint)(x);
                indices[curIdx++] = (uint)(x + 1);
                indices[curIdx++] = (uint)(x + 2);
                indices[curIdx++] = (uint)(x + 1);
                indices[curIdx++] = (uint)(x + 3);
                indices[curIdx++] = (uint)(x + 2);

                indices[curIdx++] = (uint)(x     + xVertices * 2);
                indices[curIdx++] = (uint)(x + 1 + xVertices * 2);
                indices[curIdx++] = (uint)(x + 2 + xVertices * 2);
                indices[curIdx++] = (uint)(x + 1 + xVertices * 2);
                indices[curIdx++] = (uint)(x + 3 + xVertices * 2);
                indices[curIdx++] = (uint)(x + 2 + xVertices * 2);
            }

            uint ySidesOffset = 2 * (uint)baseOffset + 4 * (uint)xVertices;
            for (uint y = ySidesOffset; y < (yVertices - 1) * 2 + ySidesOffset; y += 2)
            {
                indices[curIdx++] = (uint)(y);
                indices[curIdx++] = (uint)(y + 1);
                indices[curIdx++] = (uint)(y + 2);
                indices[curIdx++] = (uint)(y + 1);
                indices[curIdx++] = (uint)(y + 3);
                indices[curIdx++] = (uint)(y + 2);

                indices[curIdx++] = (uint)(y +     yVertices * 2);
                indices[curIdx++] = (uint)(y + 1 + yVertices * 2);
                indices[curIdx++] = (uint)(y + 2 + yVertices * 2);
                indices[curIdx++] = (uint)(y + 1 + yVertices * 2);
                indices[curIdx++] = (uint)(y + 3 + yVertices * 2);
                indices[curIdx++] = (uint)(y + 2 + yVertices * 2);
            }

            mesh.SetBufferData(vertices, indices, BufferUsageHint.DynamicDraw);

            return mesh;
        }
    }
}
