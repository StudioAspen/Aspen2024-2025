using UnityEngine;

public class PlayerFallState : PlayerBaseState
{
    public PlayerFallState(Player player, int prio) : base(player, prio)
    {

    }

    public override void OnEnter()
    {
        Debug.Log("Entering Fall State");
    }

    public override void OnExit()
    {

    }

    public override void Update()
    {
        if (player.MoveDirection.sqrMagnitude > 0f)
        {
            player.ApplyRotationToNextMovement();
            player.HandleRotation();
            player.HandleGroundedMovement();
            player.SetMovingSpeed();
        }
        else
        {
            player.HandleGroundedMovement();
            player.SetIdleSpeed();
        }

        if (player.IsGrounded)
        {
            player.ChangeState(player.PlayerIdleState, false);
        }
    }
}
