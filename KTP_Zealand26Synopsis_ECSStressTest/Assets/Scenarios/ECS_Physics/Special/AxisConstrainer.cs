using UnityEngine;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using Unity.Mathematics;

public partial struct AxisConstrainer : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        foreach (var (localTransform, velocity, physicsMass, physicsCollider, constraints) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<PhysicsVelocity>, RefRW<PhysicsMass>, RefRW<PhysicsCollider>, RefRO<Constraints>>())
        {
            var transform = localTransform.ValueRW;
            var vel = velocity.ValueRW;
            var constraint = constraints.ValueRO;
            var collider = physicsCollider.ValueRW;

            float3 newPos = transform.Position;

            if (constraint.ConstrainX)
            {
                newPos.x = constraint.fixedX;
                vel.Linear.x = 0;
            }
            if (constraint.ConstrainY)
            {
                newPos.y = constraint.fixedY;
                vel.Linear.y = 0;
            }
            newPos.z = constraint.fixedZ;
            vel.Linear.z = 0;



            localTransform.ValueRW.Position = newPos;

            velocity.ValueRW = vel;

            var mass = physicsMass.ValueRW;

            mass.InverseInertia.x = 0;
            mass.InverseInertia.y = 0;
            if (constraint.ConstrainRotation)
            {
                mass.InverseInertia.z = 0;
            }
            if(constraint.ConstrainX && constraint.ConstrainY && constraint.ConstrainZ)
            {
                mass.InverseMass = 0;
            }

            physicsMass.ValueRW = mass;
        }
    }
}

public struct Constraints : IComponentData
{
    public bool ConstrainX, ConstrainY, ConstrainZ;
    public float fixedX, fixedY, fixedZ;

    public bool ConstrainRotation;
}
