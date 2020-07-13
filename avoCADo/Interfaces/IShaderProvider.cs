namespace avoCADo
{
    public interface IShaderProvider
    {
        BufferShaderWrapper BufferShader { get; }
        ShaderWrapper DefaultShader { get; }
        TorusShaderWrapper DefaultTexturedShader { get; }
        ShaderWrapper CurveShader { get; }
        TesselationShaderWrapper SurfaceShaderBezier { get; }
        TesselationShaderWrapper SurfaceShaderDeBoor { get; }
        TesselationShaderWrapper SurfaceShaderGregory { get; }
        SimpleShaderWrapper OverlayShader { get; }
    }
}