using UnityEngine;

public class EnemyCircleState : EnemyBaseState
{
    private bool cwCircle;

    private float changeDirInterval = 0.25f;
    private float changeDirTimer;
    private float circlingRadius = 5f;

    public EnemyCircleState(Enemy enemy) : base(enemy)
    {
        this.enemy = enemy;
    }

    public override void OnEnter()
    {
        enemy.DefaultTransitionToAnimation("FlatMovement");

        enemy.SetSpeedModifier(1f);

        changeDirTimer = 0f;
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

        if (enemy.Target.TryGetComponent(out Player player))
        {
            if (player.NearbyEntities.Count > 0)
            {
                if (player.NearbyEntities.Count < enemy.CircleEntityCountThreshold)
                {
                    enemy.ChangeState(enemy.EnemyChaseState);
                }

                for (int i = 0; i < Mathf.Min(enemy.CircleEntityCountThreshold, player.NearbyEntities.Count); i++)
                {
                    if (player.NearbyEntities[i].gameObject == enemy.gameObject)
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
            enemy.NavMeshAgent.SetDestination(CalculateCircleDestination());
            cwCircle = Random.Range(0, 25) == 0 ? !cwCircle : cwCircle;
        }
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
