using UnityEngine;

public class PlayerDashState : PlayerBaseState
{
    public PlayerDashState(Player player, int prio) : base(player, prio)
    {
    }

    public override void OnEnter()
    {
        Debug.Log("Entering Dash State");

        player.CalculateTargetRotation();
        player.Dash();
        player.ChangeState(player.DefaultState, false);
    }

    public override void OnExit()
    {

    }

    public override void Update()
    {
        if (player.MoveDirection.sqrMagnitude > 0) player.CalculateTargetRotation();
        player.ResetDashDelay(); // keeps dash delay timer at 0 so that once you stop dashing, the timer goes up
    }

    public override void FixedUpdate()
    {

    }
}
