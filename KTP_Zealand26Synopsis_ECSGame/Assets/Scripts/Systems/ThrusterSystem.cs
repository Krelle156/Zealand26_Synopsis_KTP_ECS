using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

public partial struct ThrusterSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        float deltaTime = SystemAPI.Time.DeltaTime;
        foreach (var (moveComponent, thrustIntentComponent, transform) in SystemAPI.Query<RefRW<MoveComponent>, RefRO<ThrustIntentComponent>, RefRO<LocalTransform>>())
        {
            var tempMoveRO = moveComponent.ValueRO;
            moveComponent.ValueRW.currentSpeed += thrustIntentComponent.ValueRO.Thrust * transform.ValueRO.Up() * deltaTime;
            moveComponent.ValueRW.currentSpeed += thrustIntentComponent.ValueRO.ThrustLateral * transform.ValueRO.Right() * deltaTime;
            moveComponent.ValueRW.currentSpeed -= 0.1f * tempMoveRO.currentSpeed * deltaTime;  
        }
    }
}
