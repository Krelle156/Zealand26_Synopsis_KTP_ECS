using Unity.Burst;
using Unity.Entities;
using UnityEngine;


partial class OOPBridgeSystem : SystemBase
{
    protected override void OnCreate()
    {
        RequireForUpdate<MSSSystemData>();
    }

    protected override void OnUpdate()
    {
        float deltaTime = SystemAPI.Time.DeltaTime;
    }
    protected override void OnDestroy()
    {
        
    }
}
