using UnityEngine;

public class PlayerIdleState : PlayerBaseState
{
    public PlayerIdleState(Player player, int prio) : base(player, prio)
    {
        
    }

    public override void OnEnter()
    {
        Debug.Log("Entering Idle State");
    }

    public override void OnExit()
    {
        
    }

    public override void Update()
    {
        player.HandleGroundedMovement();
        player.HandleVelocity();
        player.SetIdleSpeed();

        if(player.MoveDirection.sqrMagnitude > 0)
        {
            player.ChangeState(player.PlayerMoveState, false);
        }
    }
}
