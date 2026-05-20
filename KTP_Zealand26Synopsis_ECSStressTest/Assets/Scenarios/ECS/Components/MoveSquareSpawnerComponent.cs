using Unity.Entities;
using Unity.Mathematics;

public struct MoveSquareSpawnerComponent : IComponentData
{
    public Entity prefab;

    public float3 spawnPosition;
    public float spawnRate;
    public float coolDown;
}
