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

public struct PolygonShapeArray
{
    public BlobArray<PolygonPointBlobArray> Shapes;
}

[System.Serializable]
public struct simpleShape
{
    public float2[] points;
}

public class MyPolygonColliderAuthoring : MonoBehaviour
{
    public simpleShape[] shapes;
}

public class MyPolygonBaker : Baker<MyPolygonColliderAuthoring>
{
    public override void Bake(MyPolygonColliderAuthoring authoring)
    {
        if (authoring.shapes == null || authoring.shapes[0].points.Length < 3)
        {
            authoring.shapes = new simpleShape[1];
            authoring.shapes[0] = new simpleShape { points = new float2[3] };
            authoring.shapes[0].points[0] = new float2(0, 0.5f);
            authoring.shapes[0].points[1] = new float2(-0.5f, -0.5f);
            authoring.shapes[0].points[2] = new float2(0.5f, -0.5f);
        }
        Entity entity = GetEntity(TransformUsageFlags.Dynamic);

        BlobBuilder builder = new BlobBuilder(Allocator.Temp);

        ref PolygonShapeArray polygonShapeArray = ref builder.ConstructRoot<PolygonShapeArray>();
        BlobBuilderArray<PolygonPointBlobArray> subShapeArrayBuilder = builder.Allocate(ref polygonShapeArray.Shapes, authoring.shapes.Length);


        for(int i = 0; i < authoring.shapes.Length; i++)
        {
            BlobBuilderArray<float2> pointsArray = builder.Allocate(ref subShapeArrayBuilder[i].Points, authoring.shapes[i].points.Length);

            for (int j = 0; j < authoring.shapes[i].points.Length; j++)
            {
                pointsArray[j] =  authoring.shapes[i].points[j];
            }
        }

        BlobAssetReference<PolygonShapeArray> blobAssetReference = builder.CreateBlobAssetReference<PolygonShapeArray>(Allocator.Persistent);
        builder.Dispose();

        AddBlobAsset(ref blobAssetReference, out var hash);
        AddComponent(entity, new MyPolygonColliderComponent { BlobArray = blobAssetReference });

        AddComponent(entity, new MyCircleColliderComponent { Radius = 1f });
    }
}