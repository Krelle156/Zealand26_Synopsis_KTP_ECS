using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial class CollisionSystem : SystemBase
{
    protected override void OnUpdate()
    {
        //I cannot just rely on what I see in the tutorials for what (I think) I want.
        //https://docs.unity3d.com/Packages/com.unity.entities@6.5/manual/systems-iterating-data-intro.html


        foreach(var (colliderA, transformA, entityA) in SystemAPI.Query<MyCircleColliderComponent, LocalTransform>().WithEntityAccess())
        {


            foreach (var (colliderB, transformB, entityB) in SystemAPI.Query<MyCircleColliderComponent, LocalTransform>().WithEntityAccess())
            {



                if (entityA.Index < entityB.Index)
                {
                    if(math.distance(transformA.Position, transformB.Position) < colliderA.Radius + colliderB.Radius)
                    {
                        Debug.Log($"Collision detected between {entityA} and {entityB}");
                    }

                }


            }
        }
    }



}
