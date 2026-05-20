using Unity.Entities;
using UnityEngine;

class SimpleMovementAuthoring : MonoBehaviour
{
    public float Speed = 1f;
}

class SimpleMovementAuthoringBaker : Baker<SimpleMovementAuthoring>
{
    public override void Bake(SimpleMovementAuthoring authoring)
    {
        var entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity, new SimpleMoveComponent
        {
            moveDirection = new Unity.Mathematics.float2(1, 0),
            speed = authoring.Speed
        });
    }
}
