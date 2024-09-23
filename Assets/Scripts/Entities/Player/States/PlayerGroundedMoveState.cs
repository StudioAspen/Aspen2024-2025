using UnityEngine;

public class PlayerGroundedMoveState : PlayerBaseState
{
    private float inAirTimer;

    public override void Init(Entity entity, int prio)
    {
        base.Init(entity, prio);
    }

    public override void OnEnter()
    {
        player.DefaultTransitionToAnimation("FlatMovement");
    }

    public override void OnExit()
    {

    }

    public override void Update()
    {
        player.CheckSlopeSliding();

        player.ApplyGravity();
        player.HandleGrounded();
        player.GroundedMove();

        if (player.IsFalling())
        {
            player.ChangeState(player.PlayerFallState, false);
        }
    }

    public override void FixedUpdate()
    {

    }
}
