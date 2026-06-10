using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

class MoveSquareSpawnerAuthoring : MonoBehaviour
{
    public GameObject prefab;
    public float spawnRate = 1f;
}

class MoveSquareSpawnerAuthoringBaker : Baker<MoveSquareSpawnerAuthoring>
{
    public override void Bake(MoveSquareSpawnerAuthoring authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.None);
        Entity prefabEntity = GetEntity(authoring.prefab, TransformUsageFlags.Dynamic);
        AddComponent(entity, new MoveSquareSpawnerComponent
        {
            prefab = prefabEntity,
            spawnPosition = authoring.transform.position,
            spawnRate = authoring.spawnRate,
            coolDown = 0f,
            numOfSquaresSpawned = 0
        });
    }
}
