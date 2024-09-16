using UnityEngine;

public class PlayerSlideState : PlayerBaseState
{
    private Vector3 slideDirection;

    public PlayerSlideState(Player player, int prio) : base(player, prio)
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
        player.CalculateTargetRotation();
        player.RotateTowardsTargetRotation();
        player.GroundedMove();
        player.SetMovingSpeed();

        if (!player.IsAbleToSlide()) player.ChangeState(player.DefaultState, false);
    }

    public override void FixedUpdate()
    {

    }

    public void SetSlideDirection(Vector3 dir)
    {
        slideDirection = dir;
    }
}