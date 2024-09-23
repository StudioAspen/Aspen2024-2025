﻿using UnityEngine;

public class PlayerAirborneState : PlayerBaseState
{
    public override void Init(Entity entity, int prio)
    {
        base.Init(entity, prio);
    }

    public override void OnEnter()
    {
        
    }

    public override void OnExit()
    {
        
    }

    public override void Update()
    {
        player.ApplyGravity();

        player.HandleAirborne();

        if (player.MoveDirection != Vector3.zero)
        {
            player.AccelerateToTargetSpeed(player.MovementSpeed);
            player.ApplyRotationToNextMovement();
        }
        else
        {
            player.AccelerateToTargetSpeed(0f);
        }

        player.RotateToTargetRotation();
        player.InstantlySetSpeed(player.GetGroundedVelocity().magnitude);
        player.GroundedMove();

        if (player.IsGrounded)
        {
            player.ChangeState(player.PlayerIdleState, false);
        }
    }

    public override void FixedUpdate()
    {

    }

}