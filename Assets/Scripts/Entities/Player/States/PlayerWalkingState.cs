using RPGCharacterAnims.Actions;
using UnityEngine;

public class PlayerWalkingState : PlayerBaseState
{
    public PlayerWalkingState(Player player, int prio) : base(player, prio)
    {
    }

    public override void OnEnter()
    {
        Debug.Log("Entering " + GetType().ToString());

        player.SetSpeedModifier(1f);
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

        if (player.IsSprinting)
        {
            player.ChangeState(player.PlayerRunningState, false);
        }
    }

    public override void FixedUpdate()
    {
        player.GroundedMove();
    }
}
