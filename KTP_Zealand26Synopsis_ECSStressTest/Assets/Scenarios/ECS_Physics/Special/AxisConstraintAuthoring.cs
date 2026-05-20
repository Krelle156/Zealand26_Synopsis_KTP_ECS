using UnityEngine;
using Unity.Entities;
using Unity.Physics;
using Unity.Mathematics;

public class AxisConstraintAuthoring : MonoBehaviour
{
    public bool ConstrainX, ConstrainY, ConstrainZ;
    public bool ConstrainRotation;
}

public class AxisConstraintBaker : Baker<AxisConstraintAuthoring>
{
    public override void Bake(AxisConstraintAuthoring authoring)
    {
        var entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity, new Constraints
        {
            ConstrainX = authoring.ConstrainX,
            ConstrainY = authoring.ConstrainY,
            ConstrainZ = authoring.ConstrainZ,
            fixedX = authoring.transform.position.x,
            fixedY = authoring.transform.position.y,
            fixedZ = authoring.transform.position.z,

            ConstrainRotation = authoring.ConstrainRotation,
        });

        float inverseMass = 1;
        float3 inverseInertia = new float3(1f,1f,1f);
        if(authoring.TryGetComponent(out Rigidbody body))
        {
            inverseMass = 1 / body.mass;
            inverseInertia = 1 / body.mass;
        }
        if(authoring.ConstrainX && authoring.ConstrainY && authoring.ConstrainZ)
        {
            inverseMass = 0;
        }

        if(authoring.ConstrainRotation)
        {
            inverseInertia.x = 0;
            inverseInertia.y = 0;
        }
    }

}