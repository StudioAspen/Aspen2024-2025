using UnityEngine;
using UnityEngine.AI;

public class EnemyChaseState : EnemyBaseState
{
    public EnemyChaseState(Enemy enemy) : base(enemy)
    {
        this.enemy = enemy;
    }

    public override void OnEnter()
    {
        enemy.DefaultTransitionToAnimation("FlatMovement");

        enemy.SetSpeedModifier(0.75f);
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

        FollowerCheck();

        enemy.SetDestination(enemy.Target.transform.position, true);
    }

    private void FollowerCheck()
    {
        Follower follower = enemy as Follower;
        if (follower == null) return;

        if (follower.Target.TryGetComponent(out Player player))
        {
            if (player.NearbyEntities.Count > 0)
            {
                bool qualifiedToChase = false;

                for (int i = 0; i < Mathf.Min(follower.CircleEntityCountThreshold, player.NearbyEntities.Count); i++)
                {
                    if (player.NearbyEntities[i].gameObject == enemy.gameObject)
                    {
                        qualifiedToChase = true;
                    }
                }

                if (!qualifiedToChase)
                {
                    enemy.ChangeState(follower.EnemyCircleState);
                }
            }
        }
    }

    public override void FixedUpdate()
    {

    }
}
