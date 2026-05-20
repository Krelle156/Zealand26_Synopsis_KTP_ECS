using Unity.Entities;
using UnityEngine;
using Unity.Burst;
using Unity.Transforms;

[BurstCompile]
public partial struct SquareSpawnerSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        float deltaTime = SystemAPI.Time.DeltaTime;
        float currentTime = (float)SystemAPI.Time.ElapsedTime;

        EntityCommandBuffer ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
        foreach (var spawner in SystemAPI.Query<RefRW<SquareSpawner>>())
        {
            var spawnerData = spawner.ValueRW;
            if (currentTime >= spawnerData.nextSpawnTime)
            {
                Entity newEntity = ecb.Instantiate(spawnerData.prefab);
                ecb.SetComponent(newEntity, LocalTransform.FromPosition(spawnerData.spawnPosition));
                spawnerData.nextSpawnTime = currentTime + 1f / spawnerData.spawnRate;
                spawner.ValueRW = spawnerData;

            }
        }
        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}
