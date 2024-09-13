using UnityEngine;

public class PlayerIdleState : PlayerBaseState
{
    public PlayerIdleState(Player player) : base(player)
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
        player.HandleIdleVelocity();
        player.HandleGroundedMovement();
        player.SetIdleSpeed();

        if(player.MoveDirection.sqrMagnitude > 0)
        {
            player.ChangeState(player.PlayerMoveState);
        }
    }
}
