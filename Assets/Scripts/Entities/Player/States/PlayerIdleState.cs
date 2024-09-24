using UnityEngine;

public class PlayerIdleState : PlayerBaseState
{
    public override void Init(Entity entity)
    {
        base.Init(entity);
    }

    public override void OnEnter()
    {
        player.DefaultTransitionToAnimation("FlatMovement");

        player.SetSpeedModifier(0f);
    }

    public override void OnExit()
    {
        
    }

    public override void Update()
    {
        player.ApplyGravity();

        player.AccelerateToSpeed(0f);
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
