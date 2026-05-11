using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;


public struct MoveComponent : IComponentData
{
    public float3 currentSpeed;
}
