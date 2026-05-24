using Unity.Entities;
using UnityEngine;

public struct SquareCounterSingletonComponent : IComponentData
{
    public int NumOfSquares;
}

class SquareCounterBaker : MonoBehaviour
{
    
}

class SquareCounterBakerBaker : Baker<SquareCounterBaker>
{
    public override void Bake(SquareCounterBaker authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.None);
        AddComponent(entity, new SquareCounterSingletonComponent
        {
            NumOfSquares = 0
        });
    }
}
