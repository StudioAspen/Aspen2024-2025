public abstract class PlayerBaseState : BaseState
{
    private protected Player player;

    protected PlayerBaseState(Player player, int prio) : base(player, prio)
    {
        this.player = player;
    }
}