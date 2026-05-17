using UnityEngine;
using Unity.Mathematics;
using Unity.Entities;

public class MyPolygonColliderAuthoring : MonoBehaviour
{
    public float2[] points;
}

public class MyPolygonBaker : Baker<MyPolygonColliderAuthoring>
{
    public override void Bake(MyPolygonColliderAuthoring authoring)
    {
        MyPolygonColliderComponent collider = new MyPolygonColliderComponent
        {
            points = authoring.points
        };
        
    }
}