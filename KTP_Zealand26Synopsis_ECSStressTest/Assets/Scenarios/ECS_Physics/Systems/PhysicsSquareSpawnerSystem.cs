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

        int squareCount = 0;
        float spawnRateOverride = -1f;

        if(SystemAPI.TryGetSingleton<UniversalSpawnRateComponent>(out var spawnRate))
        {
            spawnRateOverride = spawnRate.SpawnRate;
        }

        EntityCommandBuffer ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
        foreach (var spawner in SystemAPI.Query<RefRW<PhysicsSquareSpawnerComponent>>())
        {
            PhysicsSquareSpawnerComponent spawnerData = spawner.ValueRW;
            if (spawnRateOverride > 0f)
            {
                spawnerData.spawnRate = spawnRateOverride;
            }
            spawnerData.coolDown -= deltaTime;

            if (spawnerData.coolDown <= 0f)    spawnerData = SpawnSquares(ecb, spawnerData);

            spawner.ValueRW = spawnerData;
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

    [BurstCompile]
    private PhysicsSquareSpawnerComponent SpawnSquares(EntityCommandBuffer ecb, PhysicsSquareSpawnerComponent spawnerData)
    {
        Entity newEntity = ecb.Instantiate(spawnerData.prefab);
        ecb.SetComponent(newEntity, LocalTransform.FromPosition(spawnerData.spawnPosition));
        spawnerData.coolDown += 1f / spawnerData.spawnRate;
        spawnerData.numOfSquaresSpawned++;

        if (spawnerData.coolDown <= 0f)
        {
            return SpawnSquares(ecb, spawnerData);
        }

        return spawnerData;
    }
}
