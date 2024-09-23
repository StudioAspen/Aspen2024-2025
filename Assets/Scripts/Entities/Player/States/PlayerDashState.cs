using UnityEngine;
using UnityEngine.InputSystem.XR;

public class PlayerDashState : PlayerBaseState
{
    private float timer;

    private float currDashSpeed;
    private float maxSpeed;

    public override void Init(Entity entity, int prio)
    {
        base.Init(entity, prio);
        player.DashTrailSetActive(false);
    }

    public override void OnEnter()
    {
        player.Dash();

        player.DefaultTransitionToAnimation("Dash");

        timer = 0f;
        maxSpeed = player.GetMaxSpeed();

        player.ApplyRotationToNextMovement();

        player.InstantlySetSpeed(player.InitialDashVelocity);
    }

    public override void OnExit()
    {
        player.ResetYVelocity();
    }

    public override void Update()
    {
        DashUpdate();

        if (timer > player.DashDuration)
        {
            if (player.IsGrounded)
            {
                player.ChangeState(player.PlayerSprintingState, true);
                player.PlayerSprintingState.SetSprintDuration(player.SprintDurationAfterDash);
            }
            else
            {
                player.ChangeState(player.PlayerFallState, true);
            }
        }

        player.ResetDashDelay(); // keeps dash delay timer at 0 so that once you stop dashing, the timer goes up
    }

    public override void FixedUpdate()
    {
        
    }

    private void DashUpdate()
    {
        timer += Time.deltaTime;
        currDashSpeed = (player.InitialDashVelocity - maxSpeed) * (1 - Mathf.Sqrt(1 - Mathf.Pow(timer / player.DashDuration - 1, 2))) + maxSpeed;

        if (player.MoveDirection != Vector3.zero) player.ApplyRotationToNextMovement();
        player.RotateToTargetRotation();

        player.InstantlySetSpeed(currDashSpeed);
        player.GroundedMove();
    }
}
