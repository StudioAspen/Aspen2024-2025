using RPGCharacterAnims.Actions;
using UnityEngine;

public class PlayerMoveState : PlayerBaseState
{
    public PlayerMoveState(Player player, int prio) : base(player, prio)
    {
    }

    public override void OnEnter()
    {
        Debug.Log("Entering Move State");
    }

    public override void OnExit()
    {

    }

    public override void Update()
    {
        player.ApplyRotationToNextMovement();
        player.HandleVelocity();
        player.HandleRotation();
        player.HandleGroundedMovement();
        player.SetMovingSpeed();

        if (player.MoveDirection.sqrMagnitude == 0)
        {
            player.ChangeState(player.PlayerIdleState, false);
        }
    }
}
