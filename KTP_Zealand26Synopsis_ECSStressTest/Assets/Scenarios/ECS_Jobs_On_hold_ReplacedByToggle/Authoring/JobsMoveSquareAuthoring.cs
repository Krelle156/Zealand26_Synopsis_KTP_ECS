using Unity.Entities;
using UnityEngine;

public class JobsMoveSquareAuthoring : MonoBehaviour
{
    public float Speed = 1f;
}

public class JobsMoveSquareAuthoringBaker : Baker<JobsMoveSquareAuthoring>
{
    public override void Bake(JobsMoveSquareAuthoring authoring)
    {
        var entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity, new JobsMoveComponent
        {
            moveDirection = new Unity.Mathematics.float2(1, 0),
            speed = authoring.Speed
        });
    }
}
