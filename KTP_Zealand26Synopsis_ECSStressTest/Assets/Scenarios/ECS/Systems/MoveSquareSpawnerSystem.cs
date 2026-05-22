using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public struct MSSSystemData : IComponentData
{
    public int squareCount;
    public int reportInterval;
    public int nextMileStone;
}

public partial struct MoveSquareSpawnerSystem : ISystem
{
    private Random random;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        random = new Random(213);

        state.RequireForUpdate<MoveSquareSpawnerComponent>();
        state.EntityManager.AddComponent<MSSSystemData>(state.SystemHandle);
        SystemAPI.SetComponent(state.SystemHandle, new MSSSystemData
        {
            squareCount = 0,
            reportInterval = 1000
        });

        //TODO - https://docs.unity3d.com/Packages/com.unity.entities@1.0/manual/systems-data.html
        //It says to store these things in a component.

    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        float deltaTime = SystemAPI.Time.DeltaTime;
        double elapsedTime = SystemAPI.Time.ElapsedTime;

        var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
        foreach(var spawner in SystemAPI.Query<RefRW<MoveSquareSpawnerComponent>>())
        {
            if(spawner.ValueRW.coolDown <= 0f)
            {
                Entity newEntity = ecb.Instantiate(spawner.ValueRW.prefab);
                ecb.SetComponent(newEntity, LocalTransform.FromPosition(spawner.ValueRW.spawnPosition));
                ecb.SetComponent(newEntity, new SimpleMoveComponent
                {
                    moveDirection = random.NextFloat2Direction(),
                    speed = random.NextFloat(0f, 10f)
                });
                spawner.ValueRW.coolDown = 1f / spawner.ValueRW.spawnRate;


                var data = SystemAPI.GetComponent<MSSSystemData>(state.SystemHandle);
                data.squareCount++;

                SystemAPI.SetComponent(state.SystemHandle, data);
            }
            else
            {
                spawner.ValueRW.coolDown -= deltaTime;
            }

        }
        ecb.Playback(state.EntityManager);
        ecb.Dispose();

    }

}
