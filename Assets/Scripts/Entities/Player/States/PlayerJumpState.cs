using UnityEngine;

public class PlayerJumpState : PlayerBaseState
{
    private float timer;

    public PlayerJumpState(Player player) : base(player)
    {
    }

    public override void OnEnter()
    {
        Debug.Log("Entering Jump State");

        player.DefaultTransitionToAnimation("JumpingUp");

        player.Jump();

        timer = 0f;
    }

    public override void OnExit()
    {

    }

    public override void Update()
    {
        if(player.MoveDirection != Vector3.zero) player.ApplyRotationToNextMovement();

        player.RotateToTargetRotation();
        player.HandleMovingVelocity();
        player.SetGroundedSpeed(player.GetGroundedVelocity().magnitude);
        player.GroundedMove();

        timer += Time.deltaTime;
        if(timer > player.JumpTimeToFall)
        {
            player.ChangeState(player.PlayerFallState);
        }

        if (player.IsGrounded)
        {
            player.ChangeState(player.PlayerIdleState);
        }
    }

    public override void FixedUpdate()
    {

    }
}
