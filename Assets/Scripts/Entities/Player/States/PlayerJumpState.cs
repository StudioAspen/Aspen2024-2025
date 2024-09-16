using UnityEngine;

public class PlayerJumpState : PlayerBaseState
{
    public PlayerJumpState(Player player) : base(player)
    {
    }

    public override void OnEnter()
    {
        Debug.Log("Entering Jump State");

        player.TransitionToAnimation("JumpingUp");

        player.Jump();
    }

    public override void OnExit()
    {

    }

    public override void Update()
    {
        if(player.MoveDirection != Vector3.zero) player.ApplyRotationToNextMovement();

        player.RotateToTargetRotation();
        player.HandleMovingVelocity();
        player.GroundedMove();

        if (player.IsGrounded)
        {
            player.ChangeState(player.PlayerIdleState);
        }
    }

    public override void FixedUpdate()
    {

    }
}