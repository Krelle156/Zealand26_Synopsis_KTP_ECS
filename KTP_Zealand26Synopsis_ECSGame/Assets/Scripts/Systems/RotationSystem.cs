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
        foreach(var (rotationComponent, transform) in SystemAPI.Query<RefRW<RotationComponent>, RefRW<LocalTransform>>())
        {
            var rotationcomponentTemp = rotationComponent.ValueRW;
            rotationcomponentTemp.currentRotation2D += rotationcomponentTemp.rotationSpeed * SystemAPI.Time.DeltaTime;
            if(rotationcomponentTemp.currentRotation2D > math.PI * 2)
            {
                rotationcomponentTemp.currentRotation2D -= math.PI * 2;
            } else if(rotationcomponentTemp.currentRotation2D < 0)
            {
                rotationcomponentTemp.currentRotation2D += math.PI * 2;
            }

            transform.ValueRW.Rotation = quaternion.AxisAngle(zAxis, rotationcomponentTemp.currentRotation2D);
            rotationComponent.ValueRW = rotationcomponentTemp;
        }
    }

}
