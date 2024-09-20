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
        if (player.MoveDirection != Vector3.zero)
        {
            player.HandleMovingVelocity();
            player.ApplyRotationToNextMovement();
        }
        else
        {
            player.HandleIdleVelocity();
        }
            
        player.RotateToTargetRotation(); 
        player.SetGroundedSpeed(player.GetGroundedVelocity().magnitude);
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