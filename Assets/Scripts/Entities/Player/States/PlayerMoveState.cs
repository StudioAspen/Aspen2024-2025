using RPGCharacterAnims.Actions;
using UnityEngine;

public class PlayerMoveState : PlayerBaseState
{
    public PlayerMoveState(Player player) : base(player)
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
        player.HandleRotation();
        player.HandleMovingVelocity();
        player.HandleGroundedMovement();
        player.SetMovingSpeed();

        if (player.MoveDirection.sqrMagnitude == 0)
        {
            player.ChangeState(player.PlayerIdleState);
        }
    }
}
