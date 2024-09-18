using UnityEngine;
using UnityEngine.InputSystem.XR;

public class PlayerDashState : PlayerBaseState
{
    private float timer;

    private bool isDashAnimationPlaying = false;

    public override void Init(Entity entity)
    {
        base.Init(entity);
        player.DashTrailSetActive(false);
    }

    public override void OnEnter()
    {
        Debug.Log("Entering Dash State");

        player.Dash();

        isDashAnimationPlaying = false;
        if (player.IsGrounded)
        {
            player.DefaultTransitionToAnimation("Dash");
            isDashAnimationPlaying = true;
        }

        timer = 0f;

        player.ApplyRotationToNextMovement();

        player.SetGroundedSpeed(player.InitialDashVelocity);
    }

    public override void OnExit()
    {
        player.PlayerSprintingState.SetSprintDuration(player.SprintDurationAfterDash);
    }

    public override void Update()
    {
        DashUpdate();

        if(!isDashAnimationPlaying && player.IsGrounded)
        {
            player.DefaultTransitionToAnimation("FlatMovement");
            isDashAnimationPlaying = true;
        }

        if (timer > player.DashDuration)
        {
            player.ChangeState(player.PlayerSprintingState);
        }

        player.ResetDashDelay(); // keeps dash delay timer at 0 so that once you stop dashing, the timer goes up
    }

    public override void FixedUpdate()
    {
        
    }

    private void DashUpdate()
    {
        timer += Time.deltaTime;

        if(player.MoveDirection != Vector3.zero) player.ApplyRotationToNextMovement();
        
        player.RotateToTargetRotation();

        player.HandleMovingVelocity();
        player.SetGroundedSpeed(player.GetGroundedVelocity().magnitude);
        player.GroundedMove();
    }
}
