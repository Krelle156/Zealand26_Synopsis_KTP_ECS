using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class MoveComponentAuthoring : MonoBehaviour
{
    public float3 currentSpeed;
}

public class MoveComponentBaker : Baker<MoveComponentAuthoring>
{
    public override void Bake(MoveComponentAuthoring author)
    {
        Entity entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity,
            new MoveComponent
            {
                currentSpeed = author.currentSpeed
            });
        AddComponent(entity,
            new ThePlayerTag()
            );
        AddComponent(entity,
            new ThrustIntentComponent()
            {
                Thrust = 0,
                ThrustLateral = 0
            });
        AddComponent(entity,
            new RotationIntentComponent()
            {
                Rotation = 0
            });
    }
}
