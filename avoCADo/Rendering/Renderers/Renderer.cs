using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using avoCADo.Components;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace avoCADo
{
    public abstract class Renderer : MComponent, IRenderer, IDisposable
    {
        protected ShaderWrapper _shaderWrapper;
        protected Mesh _mesh;

        protected bool _shouldDispose = false;

        public Renderer(ShaderWrapper shader, VertexLayout.Type type = VertexLayout.Type.Position)
        {
            _shaderWrapper = shader;
            _mesh = new Mesh(type);
            _shouldDispose = true;
        }

        public Renderer(ShaderWrapper shader, Mesh mesh)
        {
            _shaderWrapper = shader;
            _mesh = mesh;
            _shouldDispose = false;
        }

        public override void Dispose()
        {
            if (_shouldDispose)
            {
                _mesh.Dispose();
            }
            base.Dispose();
        }

        public abstract IMeshGenerator GetGenerator();

        public void Render(ICamera camera, Matrix4 localMatrix, Matrix4 parentMatrix)
        {
            if (OwnerNode != null && OwnerNode.IsSelectable == false) return;
            if (Enabled)
            {
                _mesh.BindMesh();
                SetShader(_shaderWrapper, camera, localMatrix, parentMatrix);
                GetGenerator()?.RefreshDataPreRender();
                Draw(camera, localMatrix, parentMatrix);
                DrawTrim(camera, localMatrix, parentMatrix);
            }
        }

        private void DrawTrim(ICamera camera, Matrix4 localMatrix, Matrix4 parentMatrix)
        {
            try
            {
                if (OwnerNode != null && OwnerNode.Transform?.ParentNode != null)
                {
                    var gen = GetGenerator();
                    if (gen is ISurfaceGenerator surfGen)
                    {
                        if (surfGen.Trim)
                        {
                            foreach (var c in surfGen.Surface.BoundingCurves)
                            {
                                c.Curve.SetMode(surfGen.Surface);
                                c.Node.ForceUpdate();
                                c.Node.Render(camera, Matrix4.Identity);
                                c.Curve.SetMode(null);
                                c.Node.ForceUpdate();
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Trimming curve rendering failed!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        protected void SetShader(ShaderWrapper shaderWrapper, ICamera camera, Matrix4 localMatrix, Matrix4 parentMatrix)
        {
            shaderWrapper.Shader.Use();
            SetModelMatrix(shaderWrapper, localMatrix, parentMatrix);
        }

        protected abstract void Draw(ICamera camera, Matrix4 localMatrix, Matrix4 parentMatrix);

        private void SetModelMatrix(ShaderWrapper shader, Matrix4 localMatrix, Matrix4 parentMatrix)
        {
            var model = localMatrix * parentMatrix;
            shader.SetModelMatrix(model);
        }

        /// <summary>
        /// Use at the end of constructor
        /// </summary>
        protected abstract void SetBufferData();

    }
}
