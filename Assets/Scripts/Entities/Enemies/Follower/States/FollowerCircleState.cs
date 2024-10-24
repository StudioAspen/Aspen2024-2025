using UnityEngine;

public class FollowerCircleState : EnemyBaseState
{
    private Follower follower;

    private bool cwCircle;

    private float changeDirInterval = 0.25f;
    private float changeDirTimer;
    private float circlingRadius = 5f;

    private float canChaseTimer;
    private float canChaseCooldown = 3f;

    public FollowerCircleState(Follower enemy) : base(enemy)
    {
        follower = enemy;
    }

    public override void OnEnter()
    {
        enemy.DefaultTransitionToAnimation("FlatMovement");

        enemy.SetSpeedModifier(0.5f);

        changeDirTimer = 0f;

        canChaseTimer = 0f;
    }

    public override void OnExit()
    {

    }

    public override void Update()
    {
        if(enemy.Target == null)
        {
            enemy.ChangeState(enemy.EnemyIdleState);
            return;
        }

        canChaseTimer += Time.deltaTime;

        if (enemy.Target.TryGetComponent(out Player player))
        {
            if (player.NearbyEntities.Count > 0)
            {
                if (player.NearbyEntities.Count < follower.CircleEntityCountThreshold && canChaseTimer > canChaseCooldown)
                {
                    enemy.ChangeState(enemy.EnemyChaseState);
                }

                for (int i = 0; i < Mathf.Min(follower.CircleEntityCountThreshold, player.NearbyEntities.Count); i++)
                {
                    if (player.NearbyEntities[i] == enemy)
                    {
                        enemy.ChangeState(enemy.EnemyChaseState);
                    }
                }
            }
        }

        changeDirTimer += Time.deltaTime;

        if(changeDirTimer > changeDirInterval)
        {
            changeDirTimer = 0f;
            enemy.SetDestination(CalculateCircleDestination(), false);
            cwCircle = Random.Range(0, 25) == 0 ? !cwCircle : cwCircle;
        }

        enemy.LookAt(enemy.Target.transform.position);
    }

    public override void FixedUpdate()
    {

    }

    private Vector3 CalculateCircumferenceOffset(Vector3 center, Vector3 outside, float radius, float angleOffset)
    {
        Vector3 dirToCenter = outside - center;
        float angle = Mathf.Atan2(dirToCenter.z, dirToCenter.x) + angleOffset * Mathf.Deg2Rad;

        return new Vector3(radius * Mathf.Cos(angle), 0, radius * Mathf.Sin(angle)) + center;
    }

    private Vector3 CalculateCircleDestination()
    {
        int dirMultiplier = cwCircle ? -1 : 1;

        return CalculateCircumferenceOffset(enemy.Target.transform.position, enemy.transform.position, circlingRadius, dirMultiplier * 30f);
    }
}
