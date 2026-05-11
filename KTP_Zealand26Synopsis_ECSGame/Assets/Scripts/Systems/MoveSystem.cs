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




            transform.ValueRW.Position += moveComponent.ValueRO.currentSpeed * deltaTime;
        }
    }
}
