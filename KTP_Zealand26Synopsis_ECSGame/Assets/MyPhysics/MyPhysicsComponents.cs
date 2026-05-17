using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

public struct MyCircleColliderComponent : IComponentData
{
    public double Radius;
}

public struct MyPolygonColliderComponent : IComponentData
{
    public BlobAssetReference<PolygonPointBlobArray> BlobArray;
}

public struct MyVelocityComponent : IComponentData
{
    public double2 Value;
}
