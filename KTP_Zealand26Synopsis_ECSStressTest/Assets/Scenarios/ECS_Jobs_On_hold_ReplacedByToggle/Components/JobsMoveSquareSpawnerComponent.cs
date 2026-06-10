using Unity.Entities;
using Unity.Mathematics;
public struct JobsMoveSquareSpawnerComponent : IComponentData
{
    public Entity prefab;

    public float3 spawnPosition;
    public float secondsPerSquare; //To be converted from spawnrate (squares/second) in authoring
    public float coolDown;

    public int numOfSquaresSpawned;
}
