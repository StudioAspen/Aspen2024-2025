using UnityEngine;

public class PlayerJumpState : PlayerBaseState
{
    private float jumpStateDuration;
    private float timer;

    public PlayerJumpState(Player player, float jumpStateDuration, int prio) : base(player, prio)
    {
        this.jumpStateDuration = jumpStateDuration;
    }

    public override void OnEnter()
    {
        Debug.Log("Entering Jump State");

        player.Jump();

        timer = 0f;
    }

    public override void OnExit()
    {

    }

    public override void Update()
    {
        if (player.MoveDirection.sqrMagnitude > 0f)
        {
            player.CalculateTargetRotation();
            player.RotateTowardsTargetRotation();
            player.GroundedMove();
            player.SetMovingSpeed();
        }
        else
        {
            player.GroundedMove();
            player.SetIdleSpeed();
        }

        if (player.IsGrounded)
        {
            player.ChangeState(player.PlayerIdleState, false);
        }

        HandleJumpToFallTransition();
    }

    public override void FixedUpdate()
    {

    }

    private void HandleJumpToFallTransition()
    {
        timer += Time.deltaTime;

        if (timer > jumpStateDuration)
        {
            player.ChangeState(player.PlayerFallState, false);
        }
    }
}