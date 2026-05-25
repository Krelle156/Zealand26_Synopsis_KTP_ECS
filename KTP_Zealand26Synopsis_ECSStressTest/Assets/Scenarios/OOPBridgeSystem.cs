using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public struct UniversalSpawnRateComponent : IComponentData
{
    public float SpawnRate;
}
partial class OOPBridgeSystem : SystemBase
{
    protected override void OnCreate()
    {
        var query1 = GetEntityQuery(typeof(PhysicsSquareSpawnerComponent));
        var query2 = GetEntityQuery(typeof(MoveSquareSpawnerComponent));
        RequireAnyForUpdate(query1, query2);

        EntityManager.AddComponent<UniversalSpawnRateComponent>(SystemHandle);
    }

    protected override void OnUpdate()
    {
        float deltaTime = SystemAPI.Time.DeltaTime;
        int numOfSquares = 0;

        if(SystemAPI.TryGetSingleton<SquareCounterSingletonComponent>(out var squareCounterSingleton))
        {
            numOfSquares = squareCounterSingleton.NumOfSquares;
        }

        SharedUIController instance = SharedUIController.Instance;
        if (instance != null)
        {
            instance.ReportedNumOfSquares = numOfSquares;
        }
    }
    protected override void OnDestroy()
    {
        
    }
}
