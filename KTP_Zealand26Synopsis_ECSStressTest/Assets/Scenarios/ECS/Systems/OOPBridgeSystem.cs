using Unity.Burst;
using Unity.Entities;
using UnityEngine;

//[UpdateBefore(typeof(MoveSquareSpawnerSystem))]
public struct OOPRandom : IComponentData
{
    public uint offset;
    public float coolDown;
}

partial class OOPBridgeSystem : SystemBase
{
    protected override void OnCreate()
    {
        EntityManager.AddComponent<OOPRandom>(SystemHandle);
        SystemAPI.SetComponent(SystemHandle, new OOPRandom { offset = 0, coolDown = 0.1f });
        RequireForUpdate<MSSSystemData>();
    }

    protected override void OnUpdate()
    {
        float deltaTime = SystemAPI.Time.DeltaTime;
        MSSSystemData systemData = SystemAPI.GetSingleton<MSSSystemData>();
        if (systemData.squareCount >= systemData.nextMileStone)
        {
            Debug.Log($"Spawned {systemData.squareCount} squares!");
            systemData.nextMileStone += systemData.reportInterval;
            SystemAPI.SetSingleton(systemData);
        }
        OOPRandom random = SystemAPI.GetComponent<OOPRandom>(SystemHandle);
        if(SystemAPI.GetSingleton<OOPRandom>().coolDown > 0)
        {
            random.coolDown -= deltaTime;
        } else
        {
            random.coolDown = 0.1f;
            random.offset = (uint)(Random.value * 100000);
        }
        SystemAPI.SetComponent(SystemHandle, random);
    }
    protected override void OnDestroy()
    {
        
    }
}
