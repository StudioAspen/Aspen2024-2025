using UnityEngine;

public class PlayerSprintingState : PlayerGroundedMoveState
{
    private bool isSprintDependentOnTimer = false;
    private float duration;
    private float timer;

    public override void Init(Entity entity, int prio)
    {
        base.Init(entity, prio);
    }

    public override void OnEnter()
    {
        base.OnEnter();

        player.SetSpeedModifier(player.SprintSpeedModifier);
    }

    public override void OnExit()
    {
        isSprintDependentOnTimer = false;
        player.IsSprinting = false;
    }

    public override void Update()
    {
        base.Update();

        player.ApplyRotationToNextMovement();
        player.RotateToTargetRotation();
        player.AccelerateToTargetSpeed(player.MovementSpeed);

        if (player.MoveDirection == Vector3.zero)
        {
            player.ChangeState(player.PlayerIdleState, false);
        }

        if (player.IsSprinting) isSprintDependentOnTimer = false;

        if (isSprintDependentOnTimer)
        {
            timer += Time.deltaTime;

            if (timer > duration)
            {
                player.ChangeState(player.PlayerWalkingState, false);
            }

            return;
        }

        if (!player.IsSprinting)
        {
            player.ChangeState(player.PlayerWalkingState, false);
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
