using UnityEngine;

public class PlayerJumpState : PlayerBaseState
{
    public PlayerJumpState(Player player) : base(player)
    {
    }

    public override void OnEnter()
    {
        Debug.Log("Entering Jump State");

        player.Jump();
    }

    public override void OnExit()
    {

    }

    public override void Update()
    {
        player.ApplyRotationToNextMovement();
        player.HandleRotation();
        player.HandleMovingVelocity();
        player.HandleGroundedMovement();

        if (player.IsGrounded)
        {
            player.ChangeState(player.PlayerIdleState);
        }
    }
}