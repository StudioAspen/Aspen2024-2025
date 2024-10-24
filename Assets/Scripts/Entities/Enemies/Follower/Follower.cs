using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follower : Enemy
{
    [field: Header("Follower: Settings")]
    [field: SerializeField] public int CircleEntityCountThreshold { get; private set; } = 2;

    #region States
    public FollowerCircleState EnemyCircleState { get; private set; }
    #endregion

    protected override void OnAwake()
    {
        base.OnAwake();
    }

    protected override void OnStart()
    {
        base.OnStart();

        ChangeTeam(1);

        SetStartState(EnemyIdleState);
        SetDefaultState(EnemyIdleState);
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();
    }

    protected override void OnFixedUpdate()
    {
        base.OnFixedUpdate();
    }

    protected override void InitializeStates()
    {
        base.InitializeStates();

        EnemyCircleState = new FollowerCircleState(this);
    }
}
