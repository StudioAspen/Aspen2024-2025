using UnityEngine;

public class PlayerIdleState : PlayerBaseState
{
    public PlayerIdleState(Player player, int prio) : base(player, prio)
    {
        
    }

    public override void OnEnter()
    {
        Debug.Log("Entering Idle State");

        player.SetSpeedModifier(0f);
    }

    public override void OnExit()
    {
        
    }

    public override void Update()
    {
        if (player.MoveDirection != Vector3.zero)
        {
            player.ChangeState(player.PlayerWalkingState, false);
        }

        if (player.IsSprinting)
        {
            player.ChangeState(player.PlayerRunningState, false);
        }
    }

    public override void FixedUpdate()
    {
        
    }
}
