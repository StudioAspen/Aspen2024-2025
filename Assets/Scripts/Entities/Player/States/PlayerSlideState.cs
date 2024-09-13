using UnityEngine;

public class PlayerSlideState : PlayerBaseState
{
    private Vector3 slideDirection;

    public PlayerSlideState(Player player) : base(player)
    {
    }

    public override void OnEnter()
    {
        Debug.Log("Entering Slide State");
    }

    public override void OnExit()
    {

    }

    public override void Update()
    {
        player.IsGrounded = true;

        player.ApplySlide(slideDirection);

        // moving stuff
        player.ApplyRotationToNextMovement();
        player.HandleRotation();
        player.HandleMovingVelocity();
        player.HandleGroundedMovement();
        player.SetMovingSpeed();

        if (!player.IsAbleToSlide()) player.ChangeState(player.DefaultState);
    }

    public void SetSlideDirection(Vector3 dir)
    {
        slideDirection = dir;
    }
}