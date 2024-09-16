﻿using UnityEngine;

public class PlayerIdleState : PlayerBaseState
{
    public PlayerIdleState(Player player) : base(player)
    {
        
    }

    public override void OnEnter()
    {
        Debug.Log("Entering Idle State");

        player.DefaultTransitionToAnimation("FlatMovement");

        player.SetSpeedModifier(0f);
    }

    public override void OnExit()
    {
        
    }

    public override void Update()
    {
        player.HandleIdleVelocity();
        player.GroundedMove();

        if (player.MoveDirection != Vector3.zero && player.IsSprinting)
        {
            player.ChangeState(player.PlayerSprintingState);
            return;
        }

        if (player.MoveDirection != Vector3.zero)
        {
            player.ChangeState(player.PlayerWalkingState);
        }
    }

    public override void FixedUpdate()
    {

    }
}
