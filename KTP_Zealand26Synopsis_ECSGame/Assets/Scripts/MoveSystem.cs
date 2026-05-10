using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
public partial struct MoveSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        float deltaTime = SystemAPI.Time.DeltaTime;

        foreach (var (moveComponent, transform) in SystemAPI.Query<RefRW<MoveComponent>, RefRW<LocalTransform>>())
        {
            float2 xy = SystemAPI.GetSingleton<OmniDirectionalMovement>().Value;
            ThrustIntentComponent thrustIntent = SystemAPI.GetSingleton<ThrustIntentComponent>();
            RotationIntentComponent rotationIntent = SystemAPI.GetSingleton<RotationIntentComponent>();

            float3 forwardsStrength = transform.ValueRO.Up() * thrustIntent.Thrust;
            float3 lateralStrength = transform.ValueRO.Right() * thrustIntent.ThrustLateral;
            float rotationStrength = rotationIntent.Rotation * -0.1f;

            moveComponent.ValueRW.currentSpeed = forwardsStrength + lateralStrength;


            transform.ValueRW.Position += moveComponent.ValueRO.currentSpeed * deltaTime;
            transform.ValueRW = transform.ValueRO.RotateZ(rotationStrength * deltaTime);
        }
    }
}
