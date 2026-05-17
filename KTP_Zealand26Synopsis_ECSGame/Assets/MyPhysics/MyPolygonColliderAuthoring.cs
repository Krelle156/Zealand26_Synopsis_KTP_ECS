using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using UnityEngine;

//The following "conversation" with gemini let me fix an error. Apparently you cannot use "arrays" in struct components since they are managed (not blittable).
//At the time of writing I will admit I do not understand the word "blittable" but I assume it is close to unmanaged.
//After the assignment is done and has been appraised it is possible that the AI "conversation" will have been deleted. I cannot guarantee that I will write so here.
//https://gemini.google.com/share/c6bf0ab0deb8
//The solution is to either use BlobAssets or a Dynamic Buffer. BlobAssets seem better because I think I will end up with a set of "archetypes" of ships and blob assets are immutable.
//https://docs.unity3d.com/Packages/com.unity.entities@6.5/manual/blob-assets-intro.html

public struct PolygonPointBlobArray
{
    public BlobArray<float2> Points;
}

public class MyPolygonColliderAuthoring : MonoBehaviour
{
    public float2[] points;
}

public class MyPolygonBaker : Baker<MyPolygonColliderAuthoring>
{
    public override void Bake(MyPolygonColliderAuthoring authoring)
    {
        if (authoring.points == null || authoring.points.Length < 3)
        {
            authoring.points = new float2[3];
            authoring.points[0] = new float2(0, 0.5f);
            authoring.points[1] = new float2(-0.5f, -0.5f);
            authoring.points[2] = new float2(0.5f, -0.5f);
        }
        Entity entity = GetEntity(TransformUsageFlags.Dynamic);

        BlobBuilder builder = new BlobBuilder(Allocator.Temp);

        ref PolygonPointBlobArray polygonPointArray = ref builder.ConstructRoot<PolygonPointBlobArray>();
        BlobBuilderArray<float2> pointsArray = builder.Allocate(ref polygonPointArray.Points, authoring.points.Length);


        for (int i = 0; i < authoring.points.Length; i++)
        {
            pointsArray[i] =  authoring.points[i];
        }

        BlobAssetReference<PolygonPointBlobArray> blobAssetReference = builder.CreateBlobAssetReference<PolygonPointBlobArray>(Allocator.Persistent);
        builder.Dispose();

        AddBlobAsset(ref blobAssetReference, out var hash);
        AddComponent(entity, new MyPolygonColliderComponent { BlobArray = blobAssetReference });

        AddComponent(entity, new MyCircleColliderComponent { Radius = 1f });
    }
}