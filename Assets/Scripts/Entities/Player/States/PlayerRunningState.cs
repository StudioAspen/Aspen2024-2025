using UnityEngine;

public class PlayerRunningState : PlayerBaseState
{
    public PlayerRunningState(Player player, int prio) : base(player, prio)
    {
    }

    public override void OnEnter()
    {
        Debug.Log("Entering " + GetType().ToString());

        player.SetSpeedModifier(player.SprintSpeedModifier);
    }

    public override void OnExit()
    {

    }

    public override void Update()
    {
        player.RotateTowardsTargetRotation();
        player.CalculateTargetRotation();

        if (player.MoveDirection == Vector3.zero)
        {
            player.ChangeState(player.PlayerIdleState, false);
        }

        if (!player.IsSprinting)
        {
            player.ChangeState(player.PlayerWalkingState, false);
        }
    }

    public override void FixedUpdate()
    {
        player.GroundedMove();
    }
}
