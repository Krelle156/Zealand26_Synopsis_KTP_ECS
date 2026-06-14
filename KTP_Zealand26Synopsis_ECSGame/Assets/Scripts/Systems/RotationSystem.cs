using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

partial struct RotationSystem : ISystem
{
    float3 zAxis;

    public void OnCreate(ref SystemState state)
    {
        zAxis = new float3(0, 0, 1);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        float deltaTime = SystemAPI.Time.DeltaTime;
        foreach (var (rotationComponent, transform) in SystemAPI.Query<RefRW<RotationComponent>, RefRW<LocalTransform>>())
        {
            var rotationcomponentTemp = rotationComponent.ValueRW;
            var transformTemp = transform.ValueRW;
            
            transform.ValueRW.Rotation = math.mul(transformTemp.Rotation, quaternion.RotateZ(rotationcomponentTemp.rotationSpeed * deltaTime));
            rotationComponent.ValueRW = rotationcomponentTemp;
        }
    }

}
