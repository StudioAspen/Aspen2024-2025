using UnityEngine;

public class PlayerFallState : PlayerBaseState
{
    public PlayerFallState(Player player, int prio) : base(player, prio)
    {

    }

    public override void OnEnter()
    {
        Debug.Log("Entering Fall State");
    }

    public override void OnExit()
    {

    }

    public override void Update()
    {
        if (player.MoveDirection.sqrMagnitude > 0f)
        {
            player.CalculateTargetRotation();
            player.RotateTowardsTargetRotation();
            player.GroundedMove();
            player.SetMovingSpeed();
        }
        else
        {
            player.GroundedMove();
            player.SetIdleSpeed();
        }

        if (player.IsGrounded)
        {
            player.ChangeState(player.PlayerIdleState, false);
        }
    }

    public override void FixedUpdate()
    {

    }
}
