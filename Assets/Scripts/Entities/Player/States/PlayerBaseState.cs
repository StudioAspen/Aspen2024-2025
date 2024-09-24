using UnityEngine;

public abstract class PlayerBaseState : BaseState
{
    private protected Player player;

    public override void Init(Entity entity)
    {
        player = entity as Player;
    }
}