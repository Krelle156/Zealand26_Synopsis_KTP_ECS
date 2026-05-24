using UnityEngine;
using Unity.Entities;

public class PhysicsSquareSpawnerAuthoring : MonoBehaviour
{
    public GameObject prefab;
    public float spawnRate = 1f;
}

public class PhysicsSquareSpawnerBaker : Baker<PhysicsSquareSpawnerAuthoring>
{
    public override void Bake(PhysicsSquareSpawnerAuthoring authoring)
    {
        var entity = GetEntity(TransformUsageFlags.None);
        AddComponent(entity, new PhysicsSquareSpawnerComponent
        {
            prefab = GetEntity(authoring.prefab, TransformUsageFlags.None),
            spawnPosition = authoring.transform.position,
            spawnRate = authoring.spawnRate,
            nextSpawnTime = 0f
        });
    }
}
