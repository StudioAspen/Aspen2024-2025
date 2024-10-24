using KBCore.Refs;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : Entity
{
    [field: Header("Enemy: References")]
    [field: SerializeField, Self] public NavMeshAgent NavMeshAgent { get; protected set; }
    [SerializeField, Self] private Rigidbody rigidBody;
    [SerializeField, Self] private CapsuleCollider capsuleCollider;
    [SerializeField] private TMP_Text debugStateText;

    [field : Header("Enemy: Settings")]
    public float MovementSpeed => SpeedModifier * baseSpeed;
    private float totalSpeedModifierForAnimation;
    [field:SerializeField] public Entity Target { get; private set; }
    [field: SerializeField] public int CircleEntityCountThreshold { get; private set; }  = 2;

    #region States
    public EnemyIdleState EnemyIdleState { get; private set; }
    public EnemyChaseState EnemyChaseState { get; private set; }
    public EnemyCircleState EnemyCircleState { get; private set; }
    #endregion

    protected override void OnAwake()
    {
        base.OnAwake();

        if(Ticker.Instance != null) Ticker.Instance.OnTick.AddListener(OnTick);
    }

    protected override void OnStart()
    {
        base.OnStart();

        ChangeTeam(1);

        SetStartState(EnemyCircleState);
        SetDefaultState(EnemyCircleState);
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();

        HandleAnimations();
        HandleNavAgentSpeed();
    }

    protected override void OnFixedUpdate()
    {
        base.OnFixedUpdate();
    }

    private void LateUpdate()
    {
        DebugState();
    }

    protected virtual void OnTick()
    {
        AssignTarget();
    }

    protected override void InitializeStates()
    {
        base.InitializeStates();

        EnemyIdleState = new EnemyIdleState(this);
        EnemyChaseState = new EnemyChaseState(this);
        EnemyCircleState = new EnemyCircleState(this);
    }

    protected override void CheckGrounded()
    {
        IsGrounded = Physics.CheckSphere(transform.position + 9f * capsuleCollider.radius / 10f * Vector3.up, capsuleCollider.radius, physicsSettings.GroundLayer);
    }

    private void DebugState()
    {
        debugStateText.transform.parent.rotation = Quaternion.LookRotation(debugStateText.transform.parent.position - Camera.main.transform.position);

        debugStateText.text = $"{CurrentState.GetType()}";
    }

    private void HandleNavAgentSpeed()
    {
        NavMeshAgent.speed = MovementSpeed;
    }

    private void HandleAnimations()
    {
        totalSpeedModifierForAnimation = Mathf.Lerp(totalSpeedModifierForAnimation, SpeedModifier, NavMeshAgent.acceleration * Time.deltaTime);

        animator.SetFloat("MovementSpeed", MovementSpeed);
    }

    protected override void OnDeath()
    {
        base.OnDeath();

        if (Ticker.Instance != null) Ticker.Instance.OnTick.RemoveListener(OnTick);
    }

    public void LaunchUpwards(float magnitude)
    {
        rigidBody.AddForce(magnitude * Vector3.up, ForceMode.Impulse);
    }

    protected virtual void AssignTarget()
    {
        List<Entity> targets = GetNearbyTargets();
        if (targets.Count == 0)
        {
            Target = null;
            return;
        }

        Target = targets[0];
    }
}
