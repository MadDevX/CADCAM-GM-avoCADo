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

        protected INode _node = null;

        public void SetNode(INode node)
        {
            if (_node != null) throw new InvalidOperationException("Tried to reassign renderer's parent node");
            _node = node;
        }


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

        public virtual void Dispose()
        {
            if (_shouldDispose)
            {
                _mesh.Dispose();
            }
        }

        public abstract IMeshGenerator GetGenerator();

        public void Render(ICamera camera, Matrix4 localMatrix, Matrix4 parentMatrix)
        {
            if (_node != null && _node.IsSelectable == false) return;
            _mesh.BindMesh();
            SetShader(_shaderWrapper, camera, localMatrix, parentMatrix);
            GetGenerator()?.RefreshDataPreRender();
            Draw(camera, localMatrix, parentMatrix);
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
