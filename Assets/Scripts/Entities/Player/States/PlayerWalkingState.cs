using RPGCharacterAnims.Actions;
using UnityEngine;

public class PlayerWalkingState : PlayerBaseState
{
    public PlayerWalkingState(Player player) : base(player)
    {
    }

    public override void OnEnter()
    {
        Debug.Log("Entering Walk State");

        player.DefaultTransitionToAnimation("FlatMovement");

        player.SetSpeedModifier(1f);
    }

    public override void OnExit()
    {

    }

    public override void Update()
    {
        player.ApplyRotationToNextMovement();
        player.RotateToTargetRotation();
        player.HandleMovingVelocity();
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
