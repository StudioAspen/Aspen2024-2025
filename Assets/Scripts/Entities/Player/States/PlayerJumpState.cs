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
        if (player.MoveDirection != Vector3.zero)
        {
            player.HandleMovingVelocity();
            player.ApplyRotationToNextMovement();
        }
        else
        {
            player.HandleIdleVelocity();
        }

        player.RotateToTargetRotation();
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
