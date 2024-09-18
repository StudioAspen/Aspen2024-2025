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
        Debug.Log("Entering Fall State");

        player.TransitionToAnimation("Falling", 0.25f);
    }

    public override void OnExit()
    {

    }

    public override void Update()
    {
        if (player.MoveDirection != Vector3.zero) player.ApplyRotationToNextMovement();

        player.RotateToTargetRotation();
        player.HandleMovingVelocity();
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