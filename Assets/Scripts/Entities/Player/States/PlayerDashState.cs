using UnityEngine;

public class PlayerDashState : PlayerBaseState
{
    public PlayerDashState(Player player) : base(player)
    {
    }

    public override void OnEnter()
    {
        Debug.Log("Entering Dash State");

        player.ApplyRotationToNextMovement();
        player.Dash();
    }

    public override void OnExit()
    {

    }

    public override void Update()
    {
        if (player.MoveDirection.sqrMagnitude > 0) player.ApplyRotationToNextMovement();
        player.ResetDashDelay(); // keeps dash delay timer at 0 so that once you stop dashing, the timer goes up
    }
}
