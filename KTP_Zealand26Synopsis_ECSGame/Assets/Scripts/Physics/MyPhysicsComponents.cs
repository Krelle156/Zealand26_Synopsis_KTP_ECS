using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

public struct MyCircleColliderComponent : IComponentData
{
    public double Radius;
}

//The following "conversation" with gemini let me fix an error. Apparently you cannot use "arrays" in struct components since they are managed (not blittable).
//At the time of writing I will admit I do not understand the word "blittable" but I assume it is close to unmanaged.
//After the assignment is done and has been appraised it is possible that the AI "conversation" will have been deleted. I cannot guarantee that I will write so here.
//https://gemini.google.com/share/c6bf0ab0deb8
//The solution is to either use BlobAssets or a Dynamic Buffer. BlobAssets seem better because I think I will end up with a set of "archetypes" of ships and blob assets are immutable.
//https://docs.unity3d.com/Packages/com.unity.entities@6.5/manual/blob-assets-intro.html

public struct MyBlobOfColliderPoints
{
    public BlobArray<double2> Points;
}

public struct MyPolygonColliderComponent : IComponentData
{
    public BlobAssetReference<MyBlobOfColliderPoints> reference;
}

public struct MyVelocityComponent : IComponentData
{
    public double2 Value;
}
