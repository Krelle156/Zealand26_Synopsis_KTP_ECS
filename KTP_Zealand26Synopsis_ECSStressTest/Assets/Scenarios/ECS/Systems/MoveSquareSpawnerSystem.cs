using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public partial struct MoveSquareSpawnerSystem : ISystem
{
    //https://www.youtube.com/watch?v=s-nr9EMmhfo A mildly outdated video. It demonstrates how to use random in ECS. The main difference appears to be that entites and components are not created the same way.
    private Random random;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        random = new Random(123);

        state.RequireForUpdate<MoveSquareSpawnerComponent>();

        //https://docs.unity3d.com/Packages/com.unity.entities@1.0/manual/systems-data.html recommends storing system specifc data in a component. At least if it is to be accessible from elsewhere I think.

    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        float deltaTime = SystemAPI.Time.DeltaTime;
        double elapsedTime = SystemAPI.Time.ElapsedTime;

        int squareCount = 0;

        var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
        foreach(var spawner in SystemAPI.Query<RefRW<MoveSquareSpawnerComponent>>())
        {
            MoveSquareSpawnerComponent spawnerData = spawner.ValueRW;
            if(spawnerData.coolDown <= 0f)
            {
                Entity newEntity = ecb.Instantiate(spawnerData.prefab);
                ecb.SetComponent(newEntity, LocalTransform.FromPosition(spawnerData.spawnPosition));
                ecb.SetComponent(newEntity, new SimpleMoveComponent
                {
                    moveDirection = random.NextFloat2Direction(),
                    speed = random.NextFloat(0f, 10f)
                });
                spawnerData.coolDown = 1f / spawnerData.spawnRate;
                spawnerData.numOfSquares++;


                spawner.ValueRW = spawnerData;
            }
            else
            {
                spawnerData.coolDown -= deltaTime;
                spawner.ValueRW = spawnerData;
            }
            squareCount += spawnerData.numOfSquares;
        }

        if(SystemAPI.TryGetSingleton<SquareCounterSingletonComponent>(out var counter))
        {
            counter.NumOfSquares = squareCount;
            SystemAPI.SetSingleton(counter);
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();

    }

}
