using OpenTK;

namespace avoCADo
{
    public interface ICamera
    {
        Vector3 Target { get; }
        Vector3 Position { get; }
        float DistanceToTarget { get; }
        float Pitch { get; }
        float NearPlane { get; }
        
        Matrix4 ProjectionMatrix { get; }
        Matrix4 ViewMatrix { get; }

        Vector3 ViewPlaneVectorToWorldSpace(Vector2 vect);
        Vector3 ViewPlaneVectorToWorldSpace(float x, float y);

        void SetCameraMatrices(ShaderWrapper shaderWrapper);

        void Move(Vector3 target);
    }
}