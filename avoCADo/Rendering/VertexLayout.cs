using OpenTK.Graphics.OpenGL;

namespace avoCADo
{
    public static class VertexLayout
    {
        public enum Type
        {
            Position,
            PositionTexCoord,
            PositionNormalTexCoord
        }

        public static int Stride(Type type)
        {
            switch (type)
            {
                case Type.Position:
                    return 3;
                case Type.PositionTexCoord:
                    return 5;
                case Type.PositionNormalTexCoord:
                    return 8;
                default:
                    return -1;
            }
        }

        public static void SetLayout(int VAO, Type layoutType)
        {
            GL.BindVertexArray(VAO);
            switch (layoutType)
            {
                case Type.Position:
                    PositionLayout();
                    break;
                case Type.PositionTexCoord:
                    PositionTexCoordLayout();
                    break;
                case Type.PositionNormalTexCoord:
                    PositionNormalTexCoordLayout();
                    break;
            }
        }

        private static void PositionLayout()
        {
            var stride = Stride(Type.Position);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, stride * sizeof(float), 0 * sizeof(float));
        }

        private static void PositionTexCoordLayout()
        {
            var stride = Stride(Type.PositionTexCoord);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, stride * sizeof(float), 0 * sizeof(float));
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, stride * sizeof(float), 3 * sizeof(float));
        }

        private static void PositionNormalTexCoordLayout()
        {
            var stride = Stride(Type.PositionNormalTexCoord);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, stride * sizeof(float), 0 * sizeof(float));
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, stride * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, stride * sizeof(float), 6 * sizeof(float));
            GL.EnableVertexAttribArray(2);
        }
    }
}
