using System.Collections;
using UnityEngine;

public class PlayerFallState : PlayerBaseState
{
    public override void Init(Entity entity)
    {
        base.Init(entity);
    }

    public override void OnEnter()
    {
        player.TransitionToAnimation("Falling", 0.25f);
    }

    public override void OnExit()
    {

    }

    public override void Update()
    {
        player.ApplyGravity();

        if (player.MoveDirection != Vector3.zero)
        {
            player.AccelerateToSpeed(player.MovementSpeed);
            player.ApplyRotationToNextMovement();
        }
        else
        {
            player.AccelerateToSpeed(0f);
        }
            
        player.RotateToTargetRotation(); 
        player.InstantlySetSpeed(player.GetGroundedVelocity().magnitude);
        player.GroundedMove();

        if (player.IsGrounded)
        {
            player.ChangeState(player.PlayerIdleState);
        }
    }

    public override void FixedUpdate()
    {

    }

}