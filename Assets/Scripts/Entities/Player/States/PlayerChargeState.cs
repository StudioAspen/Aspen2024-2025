using UnityEngine;

public class PlayerChargeState : PlayerBaseState
{
    public override void Init(Entity entity)
    {
        base.Init(entity);
    }

    public override void OnEnter()
    {
        player.DefaultTransitionToAnimation("Charge");

        player.SetSpeedModifier(0);
    }

    public override void OnExit()
    {

    }

    public override void Update()
    {
        player.ApplyGravity();

        if (player.MoveDirection != Vector3.zero)
        {
            player.AccelerateToSpeed(player.MovementSpeed);
            player.ApplyRotationToNextMovement();
        }
        else
        {
            player.AccelerateToSpeed(0f);
        }

        player.RotateToTargetRotation();
        player.InstantlySetSpeed(player.GetGroundedVelocity().magnitude);
        player.GroundedMove();
    }

    public override void FixedUpdate()
    {

    }
}
