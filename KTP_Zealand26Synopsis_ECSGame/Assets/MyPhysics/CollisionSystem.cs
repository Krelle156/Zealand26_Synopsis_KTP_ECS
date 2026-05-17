using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;

public partial struct CollisionSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        //I cannot just rely on what I see in the tutorials for what (I think) I want.
        //https://docs.unity3d.com/Packages/com.unity.entities@6.5/manual/systems-iterating-data-intro.html


        foreach(var (colliderA, transformA, entityA) in SystemAPI.Query<MyPolygonColliderComponent, LocalTransform>().WithEntityAccess())
        {


            foreach (var (colliderB, transformB, entityB) in SystemAPI.Query<MyPolygonColliderComponent, LocalTransform>().WithEntityAccess())
            {



                if (entityA.Index < entityB.Index)
                {
                    ref var pointsA = ref colliderA.BlobArray.Value.Points;
                    ref var pointsB = ref colliderB.BlobArray.Value.Points;

                    FixedList512Bytes<float2> axes = new FixedList512Bytes<float2>();


                    for (int i = 0; i < pointsA.Length; i++)
                    {
                        int nextIndex = 0;
                        if(!(i == pointsA.Length - 1))
                        {
                            nextIndex = i+1;
                        }

                        axes.Add(MyMath.Math.GetNormal(transformA.Position.xy + pointsA[i], transformA.Position.xy + pointsA[nextIndex]));
                    }
                    for (int i = 0; i < pointsB.Length; i++)
                    {
                        int nextIndex = 0;
                        if (!(i == pointsB.Length - 1))
                        {
                            nextIndex = i + 1;
                        }
                        axes.Add(MyMath.Math.GetNormal(transformB.Position.xy + pointsB[i], transformB.Position.xy + pointsB[nextIndex]));
                    }

                    bool collision = true;

                    for (int i = 0; i < axes.Length; i++)
                    {
                        float2 axis = axes[i];
                        float2 projectionAMinMax = MyMath.Math.GetProjectionLengthMinMax(ref pointsA, transformA.Position.xy, axis);
                        float2 projectionBMinMax = MyMath.Math.GetProjectionLengthMinMax(ref pointsB, transformB.Position.xy, axis);
                        if(projectionAMinMax.y < projectionBMinMax.x || projectionBMinMax.y < projectionAMinMax.x)
                        {
                            collision = false;
                        }
                    }

                    if(collision)
                        Debug.Log($"Collision between {entityA} and {entityB}");


                }



            }
        }
    }



}
