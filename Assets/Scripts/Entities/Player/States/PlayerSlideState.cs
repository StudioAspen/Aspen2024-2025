using UnityEngine;

public class PlayerSlideState : PlayerGroundedMoveState
{
    private Vector3 slideDirection;

    public override void Init(Entity entity, int prio)
    {
        base.Init(entity, prio);
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
        base.Update();

        player.ApplySlide(slideDirection);

        if(player.MoveDirection != Vector3.zero)
        {
            player.AccelerateToTargetSpeed(player.MovementSpeed);
            player.ApplyRotationToNextMovement();
        }
        else
        {
            player.AccelerateToTargetSpeed(0f);
        }

        player.RotateToTargetRotation();

        if (!player.IsAbleToSlide()) player.ChangeState(player.DefaultState, true);
    }

    public override void FixedUpdate()
    {

    }

    public void SetSlideDirection(Vector3 dir)
    {
        slideDirection = dir;
    }
}
