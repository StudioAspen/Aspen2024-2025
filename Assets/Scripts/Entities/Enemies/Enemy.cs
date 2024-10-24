using KBCore.Refs;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : Entity
{
    [field: Header("Enemy: References")]
    [SerializeField, Self] private Rigidbody rigidBody;
    [SerializeField, Self] private CapsuleCollider capsuleCollider;
    [SerializeField, Child] private TMP_Text debugStateText;

    [field : Header("Enemy: Settings")]
    [field: SerializeField] public int Cost { get; protected set; }
    public float MovementSpeed => SpeedModifier * baseSpeed;
    private float totalSpeedModifierForAnimation;

    public Vector3 Destination {  get; protected set; }
    private List<Vector3> path;
    private bool lookAtPath;

    public Entity Target { get; private set; }

    #region States
    public EnemyIdleState EnemyIdleState { get; private set; }
    public EnemyChaseState EnemyChaseState { get; private set; }
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

        SetStartState(EnemyIdleState);
        SetDefaultState(EnemyIdleState);
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();

        HandleAnimations();

        MoveTowardsDestination();
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

    private void HandleAnimations()
    {
        totalSpeedModifierForAnimation = Mathf.Lerp(totalSpeedModifierForAnimation, SpeedModifier, 5f * Time.deltaTime);

        animator.SetFloat("MovementSpeed", MovementSpeed);
    }

    private List<Vector3> GetPathToDestination(Vector3 dest)
    {
        NavMeshPath path = new NavMeshPath();

        bool hasPath = NavMesh.CalculatePath(transform.position, dest, NavMesh.AllAreas, path);

        if (!hasPath) return null;
        if(path.corners.Length == 0) return null;

        return path.corners.ToList();
    }

    private void MoveTowardsDestination()
    {
        if (path == null) return;
        if(path.Count < 2) return;

        #region Debug
/*        Vector3 prevCorner = transform.position;
        foreach (Vector3 wayPoint in path)
        {
            Debug.DrawLine(prevCorner, wayPoint, Color.red);
            prevCorner = wayPoint;
        }*/
        #endregion

        Vector3 currDest = path[1];
        if (lookAtPath) LookAt(currDest);
        transform.position = Vector3.MoveTowards(transform.position, currDest, MovementSpeed * Time.deltaTime);

        if (Distance(currDest) < 0.05f)
        {
            path.RemoveAt(0);
        }
    }

    public void SetDestination(Vector3 dest, bool lookAtPath)
    {
        Destination = dest;
        path = GetPathToDestination(dest);
        this.lookAtPath = lookAtPath;
    }

    protected override void OnDeath()
    {
        base.OnDeath();

        if (Ticker.Instance != null) Ticker.Instance.OnTick.RemoveListener(OnTick);
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
