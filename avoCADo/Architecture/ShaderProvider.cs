using avoCADo.Shaders.ShaderWrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avoCADo
{
    public class ShaderProvider : IDisposable, IShaderProvider
    {
        public BufferShaderWrapper BufferShader { get; }
        public ShaderWrapper DefaultShader { get; }
        public ShaderWrapper CurveShader { get; }
        public TesselationShaderWrapper SurfaceShaderBezier { get; }
        public TesselationShaderWrapper SurfaceShaderDeBoor { get; }
        public TesselationShaderWrapper SurfaceShaderGregory { get; }
        public TorusShaderWrapper DefaultTexturedShader { get; }
        public SimpleShaderWrapper OverlayShader { get; }
        public MillableSurfaceShaderWrapper MillableSurfaceShader { get; }

        public ShaderProvider()
        {
            BufferShader = new BufferShaderWrapper(new Shader(ShaderPaths.VSQuadPath, ShaderPaths.FSQuadPath), nameof(BufferShader));
            DefaultShader = new ShaderWrapper(new Shader(ShaderPaths.VSPath, ShaderPaths.FSPath), nameof(DefaultShader));
            CurveShader = new ShaderWrapper(new Shader(ShaderPaths.VSPath, ShaderPaths.GSPath, ShaderPaths.FSPath), nameof(CurveShader));
            SurfaceShaderBezier = new TesselationShaderWrapper(new Shader(ShaderPaths.SimpleVSPath, ShaderPaths.TESCPath, ShaderPaths.TESEBezierPath, ShaderPaths.FSTexturedPath), textured:true, nameof(SurfaceShaderBezier));
            SurfaceShaderDeBoor = new TesselationShaderWrapper(new Shader(ShaderPaths.SimpleVSPath, ShaderPaths.TESCPath, ShaderPaths.TESEDeBoorPath, ShaderPaths.FSTexturedPath), textured: true, nameof(SurfaceShaderDeBoor));
            SurfaceShaderGregory = new TesselationShaderWrapper(new Shader(ShaderPaths.SimpleVSPath, ShaderPaths.TESC20Path, ShaderPaths.TESEGregoryPath, ShaderPaths.FSPath), textured: false, nameof(SurfaceShaderGregory));
            DefaultTexturedShader = new TorusShaderWrapper(new Shader(ShaderPaths.VSTexturedPath, ShaderPaths.FSTexturedPath), textured: true, nameof(DefaultTexturedShader));
            OverlayShader = new SimpleShaderWrapper(new Shader(ShaderPaths.SimpleVSPath, ShaderPaths.SimpleFSPath), nameof(OverlayShader));
            MillableSurfaceShader = new MillableSurfaceShaderWrapper(new Shader(ShaderPaths.VSMillablePath, ShaderPaths.FSMillablePath), nameof(MillableSurfaceShader));
        }

        public void UpdateShadersCameraMatrices(ICamera camera)
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
            DefaultTexturedShader.Shader.Use();
            camera.SetCameraMatrices(DefaultTexturedShader);
            MillableSurfaceShader.Shader.Use();
            camera.SetCameraMatrices(MillableSurfaceShader);
        }

        public void Dispose()
        {
            MillableSurfaceShader.Dispose();
            OverlayShader.Dispose();
            DefaultTexturedShader.Dispose();
            SurfaceShaderGregory.Dispose();
            SurfaceShaderDeBoor.Dispose();
            SurfaceShaderBezier.Dispose();
            CurveShader.Dispose();
            DefaultShader.Dispose();
            BufferShader.Dispose();
        }
    }
}
