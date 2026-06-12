using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct ThePlayerTag : IComponentData { }
public struct AutomatedOpponentTag : IComponentData { }

public struct ThrustIntentComponent : IComponentData
{
    public float Thrust;
    public float ThrustLateral;
}

public struct RotationIntentComponent : IComponentData
{
    public float Rotation;
}
