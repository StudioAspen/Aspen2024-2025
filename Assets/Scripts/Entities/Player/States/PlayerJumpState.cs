using UnityEngine;

public class PlayerJumpState : PlayerAirborneState
{
    private float timer;

    public override void Init(Entity entity, int prio)
    {
        base.Init(entity, prio);
    }

    public override void OnEnter()
    {
        base.OnEnter();

        player.DefaultTransitionToAnimation("JumpingUp");

        player.Jump();

        timer = 0f;
    }

    public override void OnExit()
    {

    }

    public override void Update()
    {
        base.Update();

        timer += Time.deltaTime;
        if(timer > player.JumpTimeToFall)
        {
            player.ChangeState(player.PlayerFallState, false);
        }
    }

    public override void FixedUpdate()
    {

    }
}
