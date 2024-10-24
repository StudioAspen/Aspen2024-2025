using UnityEngine;

public class EnemyChaseState : EnemyBaseState
{
    public EnemyChaseState(Enemy enemy) : base(enemy)
    {
        this.enemy = enemy;
    }

    public override void OnEnter()
    {
        enemy.DefaultTransitionToAnimation("FlatMovement");

        enemy.SetSpeedModifier(1f);
    }

    public override void OnExit()
    {

    }

    public override void Update()
    {
        if (enemy.Target == null)
        {
            enemy.ChangeState(enemy.EnemyIdleState);
            return;
        }

        if (enemy.Target.TryGetComponent(out Player player))
        {
            if (player.NearbyEntities.Count > 0)
            {
                bool qualifiedToChase = false;

                for (int i = 0; i < Mathf.Min(enemy.CircleEntityCountThreshold, player.NearbyEntities.Count); i++)
                {
                    if (player.NearbyEntities[i].gameObject == enemy.gameObject)
                    {
                        qualifiedToChase = true;
                    }
                }

                if (!qualifiedToChase)
                {
                    enemy.ChangeState(enemy.EnemyCircleState);
                }
            }
        }

        enemy.NavMeshAgent.destination = enemy.Target.transform.position;
    }

    public override void FixedUpdate()
    {

    }
}
