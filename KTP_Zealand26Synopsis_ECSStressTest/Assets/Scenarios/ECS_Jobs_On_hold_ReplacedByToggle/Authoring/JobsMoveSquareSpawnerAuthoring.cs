using Unity.Entities;
using UnityEngine;

public class JobsMoveSquareSpawnerAuthoring : MonoBehaviour
{
    public GameObject prefab;
    public float spawnRate = 1f;
}

public class JobsMoveSquareSpawnerAuthoringBaker : Baker<JobsMoveSquareSpawnerAuthoring>
{
    public override void Bake(JobsMoveSquareSpawnerAuthoring authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.None);
        Entity prefabEntity = GetEntity(authoring.prefab, TransformUsageFlags.Dynamic);
        AddComponent(entity, new JobsMoveSquareSpawnerComponent
        {
            prefab = prefabEntity,
            spawnPosition = authoring.transform.position,
            secondsPerSquare = 1f / authoring.spawnRate,
            coolDown = 0f,
            numOfSquaresSpawned = 0
        });
    }
}
