﻿using System;

namespace avoCADo
{
    public abstract class AbstractShaderWrapper : IDisposable
    {
        public Shader Shader { get; }
        public string Name { get; }
        public AbstractShaderWrapper(Shader shader, string name)
        {
            Shader = shader;
            Name = name;
            SetUniformLocations();
        }

        public void Dispose()
        {
            Shader.Dispose();
        }

        protected abstract void SetUniformLocations();

        protected void CheckShaderBinding()
        {
            Shader.Use();
        }
    }
}