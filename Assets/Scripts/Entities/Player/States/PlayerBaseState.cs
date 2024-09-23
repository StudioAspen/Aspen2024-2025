using UnityEngine;

public abstract class PlayerBaseState : BaseState
{
    private protected Player player;

    public override void Init(Entity entity, int prio)
    {
        base.Init(entity, prio);
        player = entity as Player;
    }
}