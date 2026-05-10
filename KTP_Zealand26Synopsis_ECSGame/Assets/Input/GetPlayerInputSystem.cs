using UnityEngine;
using Unity.Entities;

//Built on the following tutorial:
//https://www.youtube.com/watch?v=bFHvgqLUDbE

[UpdateInGroup(typeof(InitializationSystemGroup), OrderLast = true)] //Apparently this is needed to ensure we get the input first. The last bit is to ensure we do not interfere with any built in Unity stuff.
public partial class GetPlayerInputSystem : SystemBase
{
    private MyUnityActions _inputs;

    private OmniDirectionalMovement _omniDirectionalMovement = new OmniDirectionalMovement
    {
        Value = new Vector2(0, 0)
    };
    private ThrustIntentComponent _thrustComponent = new ThrustIntentComponent
    {
        Thrust = 0,
        ThrustLateral = 0
    };
    private RotationIntentComponent _rotationComponent = new RotationIntentComponent
    {
        Rotation = 0
    };
    private Entity _thePlayerEntity;

    protected override void OnCreate()
    {
        _inputs = new MyUnityActions();
        RequireForUpdate<ThePlayerTag>();
        //If I want to be able to try to move both forwards/backwards and laterally without both systems technically existing I'd need to make a new system. Or I need to get a better grip of ECS.
        //I think this is sensible though.
        //https://discussions.unity.com/t/how-does-requireforupdate-work-under-the-hood/935624 or not? I am not sure when to use it.
        RequireForUpdate<ThrustIntentComponent>();
        RequireForUpdate<RotationIntentComponent>();    
    }

    protected override void OnStartRunning()
    {
        _inputs.Enable();
        _thePlayerEntity = SystemAPI.GetSingletonEntity<ThePlayerTag>();
    }

    protected override void OnStopRunning()
    {
        _inputs.Disable();
        _thePlayerEntity = Entity.Null;
    }

    protected override void OnUpdate()
    {
        _omniDirectionalMovement.Value = _inputs.PlayerShip.OmniDirectionalThrust.ReadValue<Vector2>();

        _thrustComponent.Thrust = _inputs.PlayerShip.MainThrust.ReadValue<float>();
        _thrustComponent.ThrustLateral = _inputs.PlayerShip.LateralThrust.ReadValue<float>();
        _rotationComponent.Rotation = _inputs.PlayerShip.Turn.ReadValue<float>();

        SystemAPI.SetSingleton(_thrustComponent);
        SystemAPI.SetSingleton(_rotationComponent);
        SystemAPI.SetSingleton(_omniDirectionalMovement);
    }
}
