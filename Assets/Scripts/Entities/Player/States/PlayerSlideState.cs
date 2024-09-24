using UnityEngine;

public class PlayerSlideState : PlayerBaseState
{
    private Vector3 slideDirection;

    public override void Init(Entity entity)
    {
        base.Init(entity);
    }

    public override void OnEnter()
    {
        player.DefaultTransitionToAnimation("Falling");
    }

    public override void OnExit()
    {

    }

    public override void Update()
    {
        player.IsGrounded = true;

        player.ApplyGravity();
        player.ApplySlide(slideDirection);

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

        if (!player.IsAbleToSlide()) player.ChangeState(player.DefaultState);
    }

    public override void FixedUpdate()
    {

    }

    public void SetSlideDirection(Vector3 dir)
    {
        slideDirection = dir;
    }
}
