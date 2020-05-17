using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace avoCADo
{
    public abstract class Renderer : IRenderer, IDisposable
    {
        protected ShaderWrapper _shaderWrapper;
        protected Mesh _mesh;

        protected bool _shouldDispose = false;

        public Renderer(ShaderWrapper shader)
        {
            _shaderWrapper = shader;
            _mesh = new Mesh();
            _shouldDispose = true;
        }

        public Renderer(ShaderWrapper shader, Mesh mesh)
        {
            _shaderWrapper = shader;
            _mesh = mesh;
            _shouldDispose = false;
        }

        public virtual void Dispose()
        {
            if (_shouldDispose)
            {
                _mesh.Dispose();
            }
        }

        public abstract IMeshGenerator GetGenerator();

        public void Render(Camera camera, Matrix4 localMatrix, Matrix4 parentMatrix)
        {
            _mesh.BindMesh();
            SetShader(_shaderWrapper, camera, localMatrix, parentMatrix);
            GetGenerator()?.RefreshDataPreRender();
            Draw(camera, localMatrix, parentMatrix);
        }

        protected void SetShader(ShaderWrapper shaderWrapper, Camera camera, Matrix4 localMatrix, Matrix4 parentMatrix)
        {
            shaderWrapper.Shader.Use();
            SetModelMatrix(shaderWrapper, localMatrix, parentMatrix);
            camera.SetCameraMatrices(shaderWrapper);
        }

        protected abstract void Draw(Camera camera, Matrix4 localMatrix, Matrix4 parentMatrix);

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
