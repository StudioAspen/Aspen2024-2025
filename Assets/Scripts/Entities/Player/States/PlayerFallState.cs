using System.Collections;
using UnityEngine;

public class PlayerFallState : PlayerAirborneState
{
    public override void Init(Entity entity, int prio)
    {
        base.Init(entity, prio);
    }

    public override void OnEnter()
    {
        player.TransitionToAnimation("Falling", 0.25f);

        player.StartFall();
    }

    public override void OnExit()
    {

    }

    public override void Update()
    {
        base.Update();
    }

    public override void FixedUpdate()
    {

    }

}
