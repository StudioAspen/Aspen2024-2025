using UnityEngine;

public class PlayerIdleState : PlayerGroundedMoveState
{
    public override void Init(Entity entity, int prio)
    {
        base.Init(entity, prio);
    }

    public override void OnEnter()
    {
        base.OnEnter();

        player.SetSpeedModifier(0f);
    }

    public override void OnExit()
    {
        
    }

    public override void Update()
    {
        base.Update();

        player.AccelerateToTargetSpeed(0f);

        if (player.MoveDirection != Vector3.zero && player.IsSprinting)
        {
            player.ChangeState(player.PlayerSprintingState, false);
            return;
        }

        if (player.MoveDirection != Vector3.zero)
        {
            player.ChangeState(player.PlayerWalkingState, false);
        }
    }

    public override void FixedUpdate()
    {

    }
}
