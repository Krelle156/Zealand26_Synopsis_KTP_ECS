using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class CollisionTestAuthoring : MonoBehaviour
{
    public bool isPlayer = false;
    public float3 currentSpeed;
}

public class CollisionTestBaker : Baker<CollisionTestAuthoring>
{
    public override void Bake(CollisionTestAuthoring author)
    {
        Entity entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity,
            new MoveComponent
            {
                currentSpeed = author.currentSpeed
            });
        AddComponent(entity,
            new RotationComponent
            {
                rotationSpeed = 0
            });
        if (author.isPlayer)
        {
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
}