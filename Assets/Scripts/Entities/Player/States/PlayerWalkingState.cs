using RPGCharacterAnims.Actions;
using UnityEngine;

public class PlayerWalkingState : PlayerBaseState
{
    public override void Init(Entity entity)
    {
        base.Init(entity);
    }

    public override void OnEnter()
    {
        player.DefaultTransitionToAnimation("FlatMovement");

        player.SetSpeedModifier(1f);
    }

    public override void OnExit()
    {

    }

    public override void Update()
    {
        player.ApplyGravity();

        player.ApplyRotationToNextMovement();
        player.RotateToTargetRotation();
        player.AccelerateToSpeed(player.MovementSpeed);
        player.GroundedMove();

        if (player.MoveDirection == Vector3.zero)
        {
            player.ChangeState(player.PlayerIdleState);
        }

        if (player.IsSprinting)
        {
            player.ChangeState(player.PlayerSprintingState);
        }
    }

    public override void FixedUpdate()
    {

    }
}
