using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct ThePlayerTag : IComponentData { }
public struct AutomatedOpponentTag : IComponentData { }

public struct OmniDirectionalMovement : IComponentData
{
    public float2 Value;
}

public struct ThrustIntentComponent : IComponentData
{
    public float Thrust;
    public float ThrustLateral;
}

public struct RotationIntentComponent : IComponentData
{
    public float Rotation;
}
