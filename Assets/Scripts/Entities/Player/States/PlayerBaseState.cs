public abstract class PlayerBaseState : BaseState
{
    private protected Player player;

    protected PlayerBaseState(Player player) : base(player)
    {
        this.player = player;
    }
}