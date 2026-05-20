using Unity.Entities;
using Unity.Mathematics;

public struct SimpleMoveComponent : IComponentData
{
    public float2 moveDirection;
    public float speed;
}
