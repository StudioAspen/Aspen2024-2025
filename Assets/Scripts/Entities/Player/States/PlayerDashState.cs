using UnityEngine;
using UnityEngine.InputSystem.XR;

public class PlayerDashState : PlayerBaseState
{
    private float timer;

    private float currDashSpeed;
    private float maxSpeed;

    private bool isDashAnimationPlaying = false;

    public PlayerDashState(Player player) : base(player)
    {
        player.DashTrailSetActive(false);
    }

    public override void OnEnter()
    {
        Debug.Log("Entering Dash State");

        player.Dash();

        isDashAnimationPlaying = false;
        if (player.IsGrounded)
        {
            player.TransitionToAnimation("Dash");
            isDashAnimationPlaying = true;
        }
           

        timer = 0f;

        currDashSpeed = player.InitialDashVelocity;

        maxSpeed = player.GetMaxSpeed();

        player.ApplyRotationToNextMovement();

        player.DashTrailSetActive(true);
    }

    public override void OnExit()
    {
        player.PlayerSprintingState.SetSprintDuration(player.SprintDurationAfterDash);
        player.DashTrailSetActive(false);
    }

    public override void Update()
    {
        DashUpdate();

        if(!isDashAnimationPlaying && player.IsGrounded)
        {
            player.TransitionToAnimation("FlatMovement");
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

        currDashSpeed = (player.InitialDashVelocity - maxSpeed) * (1 - Mathf.Sqrt(1 - Mathf.Pow(timer / player.DashDuration - 1, 2))) + maxSpeed;

        player.SetGroundedSpeed(currDashSpeed);
        player.GroundedMove();
    }
}
