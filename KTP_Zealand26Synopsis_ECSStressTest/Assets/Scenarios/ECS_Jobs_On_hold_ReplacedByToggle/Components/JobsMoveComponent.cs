using Unity.Entities;
using Unity.Mathematics;

public struct JobsMoveComponent : IComponentData
{
    public float2 moveDirection;
    public float speed;
}
