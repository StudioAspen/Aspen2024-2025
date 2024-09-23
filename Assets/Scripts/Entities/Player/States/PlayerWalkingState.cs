using RPGCharacterAnims.Actions;
using UnityEngine;

public class PlayerWalkingState : PlayerGroundedMoveState
{
    public override void Init(Entity entity, int prio)
    {
        base.Init(entity, prio);
    }

    public override void OnEnter()
    {
        base.OnEnter();

        player.SetSpeedModifier(1f);
    }

    public override void OnExit()
    {

    }

    public override void Update()
    {
        base.Update();

        player.ApplyRotationToNextMovement();
        player.RotateToTargetRotation();
        player.AccelerateToTargetSpeed(player.MovementSpeed);

        if (player.MoveDirection == Vector3.zero)
        {
            player.ChangeState(player.PlayerIdleState, false);
        }

        if (player.IsSprinting)
        {
            player.ChangeState(player.PlayerSprintingState, false);
        }
    }

    public override void FixedUpdate()
    {

    }
}
