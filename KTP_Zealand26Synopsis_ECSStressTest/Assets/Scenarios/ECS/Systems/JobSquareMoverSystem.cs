using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Burst;
using Unity.Collections;

[BurstCompile]
public partial struct JobSquareMoverSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<SimpleMoveComponent>();
        state.Enabled = false;
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        SquareMoveJob myFirstEverUnityJobYay = new SquareMoveJob
        {
            deltaTime = SystemAPI.Time.DeltaTime
        };

        myFirstEverUnityJobYay.ScheduleParallel();

    }
}

[BurstCompile]
public partial struct SquareMoveJob : IJobEntity
{
    //https://www.youtube.com/watch?v=6gFyoMoa8dM this guy explained the ReadOnly as important? He did work with a slightly different kind of Unity "job".
    //TODO - understand why we use this, beyond just it being safe for parallelism in some abstract way.
    [ReadOnly] public float deltaTime;

    void Execute(ref SimpleMoveComponent move, ref LocalTransform transform)
    {
        if(move.speed > 0)
        {
            transform.Position += new float3(-move.moveDirection * move.speed, 0) * deltaTime;
        }
    }
}
