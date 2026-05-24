using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Burst;
[BurstCompile]
public partial struct SquareMoverSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
        foreach (var (move, transform) in SystemAPI.Query<RefRW<SimpleMoveComponent>, RefRW<LocalTransform>>())
        {
            float speed = move.ValueRW.speed;
            float deltaTime = SystemAPI.Time.DeltaTime;
            if (speed > 0)
            {
                transform.ValueRW.Position += new Unity.Mathematics.float3(move.ValueRW.moveDirection * speed, 0) * deltaTime;
            }
            move.ValueRW.speed = speed;
        }

    }
}
