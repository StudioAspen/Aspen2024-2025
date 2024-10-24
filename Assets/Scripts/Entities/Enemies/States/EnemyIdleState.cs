public class EnemyIdleState : EnemyBaseState
{
    public EnemyIdleState(Enemy enemy) : base(enemy)
    {
        this.enemy = enemy;
    }

    public override void OnEnter()
    {
        enemy.DefaultTransitionToAnimation("FlatMovement");

        enemy.SetSpeedModifier(0f);
    }

    public override void OnExit()
    {

    }

    public override void Update()
    {
        if (enemy.Target != null)
        {
            enemy.ChangeState(enemy.EnemyChaseState);
        }
    }

    public override void FixedUpdate()
    {

    }
}
