using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo
{
    public class ShaderProvider : IDisposable
    {
        public readonly BufferShaderWrapper BufferShader;
        public readonly ShaderWrapper DefaultShader;
        public readonly ShaderWrapper CurveShader;
        public readonly TesselationShaderWrapper SurfaceShader;
        public readonly SimpleShaderWrapper OverlayShader;

        public ShaderProvider()
        {
            BufferShader = new BufferShaderWrapper(new Shader(ShaderPaths.VSQuadPath, ShaderPaths.FSQuadPath));
            DefaultShader = new ShaderWrapper(new Shader(ShaderPaths.VSPath, ShaderPaths.FSPath));
            CurveShader = new ShaderWrapper(new Shader(ShaderPaths.VSPath, ShaderPaths.GSPath, ShaderPaths.FSPath));
            SurfaceShader = new TesselationShaderWrapper(new Shader(ShaderPaths.SimpleVSPath, ShaderPaths.TESCPath, ShaderPaths.TESEPath, ShaderPaths.FSPath));
            OverlayShader = new SimpleShaderWrapper(new Shader(ShaderPaths.SimpleVSPath, ShaderPaths.SimpleFSPath));
        }

        public void UpdateShadersCameraMatrices(Camera camera)
        {
            DefaultShader.Shader.Use();
            camera.SetCameraMatrices(DefaultShader);
            CurveShader.Shader.Use();
            camera.SetCameraMatrices(CurveShader);
            SurfaceShader.Shader.Use();
            camera.SetCameraMatrices(SurfaceShader);
        }

        public void Dispose()
        {
            OverlayShader.Dispose();
            SurfaceShader.Dispose();
            CurveShader.Dispose();
            DefaultShader.Dispose();
            BufferShader.Dispose();
        }
    }
}
