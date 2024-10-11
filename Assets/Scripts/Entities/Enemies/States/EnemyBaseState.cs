public abstract class EnemyBaseState : BaseState
{
    private protected Enemy enemy;

    public EnemyBaseState(Enemy enemy) : base(enemy)
    {
        this.enemy = enemy;
    }
}