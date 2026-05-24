using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

public struct PhysicsSquareSpawnerComponent : IComponentData
{
    public Entity prefab;

    public float3 spawnPosition;
    public float spawnRate;
    public float nextSpawnTime;

    public int numOfSquaresSpawned;
}
