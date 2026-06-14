using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;
using Unity.Burst;

[BurstCompile]
public partial struct CollisionSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityQuery query = SystemAPI.QueryBuilder()
            .WithAll<MyPolygonColliderComponent, LocalTransform>()
            .Build();

        var allEntities = query.ToEntityArray(Allocator.TempJob);
        var allTransforms = query.ToComponentDataArray<LocalTransform>(Allocator.TempJob);
        var allColliders = query.ToComponentDataArray<MyPolygonColliderComponent>(Allocator.TempJob);

        var collisionJob = new MyCollisisonJob
        {
            OtherEntities = allEntities,
            OtherTransforms = allTransforms,
            OtherColliders = allColliders
        };

        state.Dependency = collisionJob.Schedule(state.Dependency);

        allEntities.Dispose(state.Dependency);
        allTransforms.Dispose(state.Dependency);
        allColliders.Dispose(state.Dependency);
    }

}

public partial struct MyCollisisonJob : IJobEntity
{
    [ReadOnly] public NativeArray<Entity> OtherEntities;
    [ReadOnly] public NativeArray<LocalTransform> OtherTransforms;
    [ReadOnly] public NativeArray<MyPolygonColliderComponent> OtherColliders;

    public void Execute(Entity entityA, in LocalTransform transformA, in MyPolygonColliderComponent colliderA)
    {
        for(int i=0; i < OtherEntities.Length; i++)
        {
            Entity entityB = OtherEntities[i];
            if(entityA.Index < entityB.Index)
            {
                LocalTransform transformB = OtherTransforms[i];
                MyPolygonColliderComponent colliderB = OtherColliders[i];
                for(int j = 0; j < colliderA.BlobArray.Value.Shapes.Length; j++)
                {
                    for(int k = 0; k < colliderB.BlobArray.Value.Shapes.Length; k++)
                    {
                        if(CheckCollision(colliderA, transformA, colliderB, transformB, j, k))
                        {
                            Debug.Log($"Collision between {entityA} and {entityB}");
                        }
                    }
                }

            }
        }
    }

    [BurstCompile]
    private bool CheckCollision(MyPolygonColliderComponent colliderA, LocalTransform transformA, MyPolygonColliderComponent colliderB, LocalTransform transformB, int shapeAIndex, int shapeBIndex)
    {
        ref var pointsA = ref colliderA.BlobArray.Value.Shapes[shapeAIndex].Points;
        ref var pointsB = ref colliderB.BlobArray.Value.Shapes[shapeBIndex].Points;
        FixedList512Bytes<float2> axes = new FixedList512Bytes<float2>();
        FixedList4096Bytes<float2> worldPointsA = new FixedList4096Bytes<float2>();
        FixedList4096Bytes<float2> worldPointsB = new FixedList4096Bytes<float2>();

        for (int i = 0; i < pointsA.Length; i++)
        {
            float2 rotatedPoint = math.rotate(transformA.Rotation, new float3(pointsA[i].xy, 0)).xy;
            worldPointsA.Add(transformA.Position.xy + rotatedPoint);
        }
        for (int i = 0; i < pointsB.Length; i++)
        {
            float2 rotatedPoint = math.rotate(transformB.Rotation, new float3(pointsB[i].xy, 0)).xy;
            worldPointsB.Add(transformB.Position.xy + rotatedPoint);
        }

        for (int i = 0; i < worldPointsA.Length; i++)
        {
            int nextIndex = 0;
            if (!(i == worldPointsA.Length - 1))
            {
                nextIndex = i + 1;
            }

            axes.Add(MyMath.Math.GetNormal(worldPointsA[i], worldPointsA[nextIndex]));
        }
        for (int i = 0; i < worldPointsB.Length; i++)
        {
            int nextIndex = 0;
            if (!(i == worldPointsB.Length - 1))
            {
                nextIndex = i + 1;
            }
            axes.Add(MyMath.Math.GetNormal(worldPointsB[i], worldPointsB[nextIndex]));
        }

        for (int i = 0; i < axes.Length; i++)
        {
            float2 axis = axes[i];
            float2 projectionAMinMax = MyMath.Math.GetProjectionLengthMinMax(worldPointsA, axis);
            float2 projectionBMinMax = MyMath.Math.GetProjectionLengthMinMax(worldPointsB, axis);
            if (projectionAMinMax.y < projectionBMinMax.x || projectionBMinMax.y < projectionAMinMax.x)
            {
                return false;
            }
        }
        return true;
    }
}
