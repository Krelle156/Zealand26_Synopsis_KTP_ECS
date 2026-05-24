using Unity.Entities;
using UnityEngine;
using Unity.Burst;
using Unity.Transforms;

[BurstCompile]
public partial struct PhysicsSquareSpawnerSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PhysicsSquareSpawnerComponent>();
    }   

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        float deltaTime = SystemAPI.Time.DeltaTime;
        float currentTime = (float)SystemAPI.Time.ElapsedTime;
        int squareCount = 0;

        EntityCommandBuffer ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
        foreach (var spawner in SystemAPI.Query<RefRW<PhysicsSquareSpawnerComponent>>())
        {
            var spawnerData = spawner.ValueRW;
            if (currentTime >= spawnerData.nextSpawnTime)
            {
                Entity newEntity = ecb.Instantiate(spawnerData.prefab);
                ecb.SetComponent(newEntity, LocalTransform.FromPosition(spawnerData.spawnPosition));
                spawnerData.nextSpawnTime = currentTime + 1f / spawnerData.spawnRate;
                spawnerData.numOfSquaresSpawned++;
                spawner.ValueRW = spawnerData;

            }
            squareCount += spawnerData.numOfSquaresSpawned;
        }

        if (SystemAPI.TryGetSingleton<SquareCounterSingletonComponent>(out var counter))
        {
            counter.NumOfSquares = squareCount;
            SystemAPI.SetSingleton(counter);
        }


        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}
