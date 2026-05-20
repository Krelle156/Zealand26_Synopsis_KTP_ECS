using Unity.Entities;
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
        AddComponent(entity, new MoveSquareSpawnerComponent
        {
            prefab = GetEntity(authoring.prefab, TransformUsageFlags.None),
            spawnPosition = authoring.transform.position,
            spawnRate = authoring.spawnRate,
            coolDown = 0f
        });
    }
}
