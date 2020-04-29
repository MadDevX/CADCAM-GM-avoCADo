using System;

namespace avoCADo
{
    public abstract class AbstractShaderWrapper : IDisposable
    {
        public Shader Shader { get; }

        public AbstractShaderWrapper(Shader shader)
        {
            Shader = shader;

            SetUniformLocations();
        }

        public void Dispose()
        {
            Shader.Dispose();
        }

        protected abstract void SetUniformLocations();

        protected void CheckShaderBinding()
        {
            //TODO: check if it's possible to check currently bound shader program
            Shader.Use();
        }
    }
}