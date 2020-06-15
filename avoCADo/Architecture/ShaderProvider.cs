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
        public readonly TesselationShaderWrapper SurfaceShaderBezier;
        public readonly TesselationShaderWrapper SurfaceShaderDeBoor;
        public readonly TesselationShaderWrapper SurfaceShaderGregory;
        public readonly SimpleShaderWrapper OverlayShader;

        public ShaderProvider()
        {
            BufferShader = new BufferShaderWrapper(new Shader(ShaderPaths.VSQuadPath, ShaderPaths.FSQuadPath), nameof(BufferShader));
            DefaultShader = new ShaderWrapper(new Shader(ShaderPaths.VSPath, ShaderPaths.FSPath), nameof(DefaultShader));
            CurveShader = new ShaderWrapper(new Shader(ShaderPaths.VSPath, ShaderPaths.GSPath, ShaderPaths.FSPath), nameof(CurveShader));
            SurfaceShaderBezier = new TesselationShaderWrapper(new Shader(ShaderPaths.SimpleVSPath, ShaderPaths.TESCPath, ShaderPaths.TESEBezierPath, ShaderPaths.FSPath), nameof(SurfaceShaderBezier));
            SurfaceShaderDeBoor = new TesselationShaderWrapper(new Shader(ShaderPaths.SimpleVSPath, ShaderPaths.TESCPath, ShaderPaths.TESEDeBoorPath, ShaderPaths.FSPath), nameof(SurfaceShaderDeBoor));
            SurfaceShaderGregory = new TesselationShaderWrapper(new Shader(ShaderPaths.SimpleVSPath, ShaderPaths.TESC20Path, ShaderPaths.TESEGregoryPath, ShaderPaths.FSPath), nameof(SurfaceShaderGregory));
            OverlayShader = new SimpleShaderWrapper(new Shader(ShaderPaths.SimpleVSPath, ShaderPaths.SimpleFSPath), nameof(OverlayShader));
        }

        public void UpdateShadersCameraMatrices(Camera camera)
        {
            DefaultShader.Shader.Use();
            camera.SetCameraMatrices(DefaultShader);
            CurveShader.Shader.Use();
            camera.SetCameraMatrices(CurveShader);
            SurfaceShaderBezier.Shader.Use();
            camera.SetCameraMatrices(SurfaceShaderBezier);
            SurfaceShaderDeBoor.Shader.Use();
            camera.SetCameraMatrices(SurfaceShaderDeBoor);
            SurfaceShaderGregory.Shader.Use();
            camera.SetCameraMatrices(SurfaceShaderGregory);
        }

        public void Dispose()
        {
            OverlayShader.Dispose();
            SurfaceShaderGregory.Dispose();
            SurfaceShaderDeBoor.Dispose();
            SurfaceShaderBezier.Dispose();
            CurveShader.Dispose();
            DefaultShader.Dispose();
            BufferShader.Dispose();
        }
    }
}
