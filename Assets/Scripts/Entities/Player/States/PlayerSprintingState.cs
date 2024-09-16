using UnityEngine;

public class PlayerSprintingState : PlayerBaseState
{
    private bool isSprintDependentOnTimer = false;
    private float duration;
    private float timer;

    public PlayerSprintingState(Player player) : base(player)
    {
    }

    public override void OnEnter()
    {
        Debug.Log("Entering Sprint State");

        player.DefaultTransitionToAnimation("FlatMovement");

        player.SetSpeedModifier(player.SprintSpeedModifier);
    }

    public override void OnExit()
    {
        isSprintDependentOnTimer = false;
        player.IsSprinting = false;
    }

    public override void Update()
    {
        player.ApplyRotationToNextMovement();
        player.RotateToTargetRotation();
        player.HandleMovingVelocity();
        player.GroundedMove();

        if (player.MoveDirection == Vector3.zero)
        {
            player.ChangeState(player.PlayerIdleState);
        }

        if (player.IsSprinting) isSprintDependentOnTimer = false;

        if (isSprintDependentOnTimer)
        {
            timer += Time.deltaTime;

            if (timer > duration)
            {
                player.ChangeState(player.PlayerWalkingState);
            }

            return;
        }

        if (!player.IsSprinting)
        {
            player.ChangeState(player.PlayerWalkingState);
        }
    }

    public override void FixedUpdate()
    {

    }

    public void SetSprintDuration(float d)
    {
        timer = 0f;
        duration = d;
        isSprintDependentOnTimer = true;
    }
}
