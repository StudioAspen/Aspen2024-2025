using UnityEngine;

public class PlayerJumpState : PlayerBaseState
{
    private float timer;

    public override void Init(Entity entity)
    {
        base.Init(entity);
    }

    public override void OnEnter()
    {
        player.DefaultTransitionToAnimation("JumpingUp");

        player.Jump();

        timer = 0f;
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
