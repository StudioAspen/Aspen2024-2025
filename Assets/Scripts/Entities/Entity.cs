using KBCore.Refs;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Entity : MonoBehaviour
{
    [Header("Entity: References")]
    [SerializeField, Self] private protected Animator animator;
    [SerializeField] private protected GlobalPhysicsSettings physicsSettings;
    [SerializeField, Anywhere] private HitNumbers hitNumberPrefab;

    [field: Header("Entity: Settings")]
    [field: SerializeField] public int CurrentHealth { get; private set; }
    [field: SerializeField] public int MaxHealth { get; private set; }
    [field: SerializeField] public int Level { get; private set; }
    [HideInInspector] public bool IsGrounded;

    [SerializeField] protected float baseSpeed = 3f;
    public float SpeedModifier { get; protected set; } = 1f;
    [SerializeField] protected private Vector3 velocity;
    [SerializeField] private protected float targetDetectionRadius = 10f;
    protected private float inAirTimer;
    protected private bool fallVelocityApplied;

    public int Team { get; private set; }

    #region States
    public BaseState CurrentState { get; private set; }
    public BaseState DefaultState { get; private set; }
    public EntityEmptyState EntityEmptyState { get; private set; }
    public EntityHitState EntityHitState { get; private set; }
    public EntityDeathState EntityDeathState { get; private set; }
    #endregion

    private void OnValidate()
    {
        this.ValidateRefs();
    }

    private void Awake()
    {
        OnAwake();
    }

    protected virtual void OnAwake()
    {
        InitializeStates();
    }

    private void Start()
    {
        OnStart();
    }

    protected virtual void OnStart()
    {
        SetDefaultState(EntityEmptyState);
        SetStartState(EntityEmptyState);

        IgnoreMyOwnColliders();

        CurrentHealth = MaxHealth;
    }

    private void Update()
    {
        OnUpdate();   
    }

    protected virtual void OnUpdate()
    {
        CurrentState?.Update();

        CheckGrounded();
    }

    private void FixedUpdate()
    {
        OnFixedUpdate();
    }

    protected virtual void OnFixedUpdate()
    {
        CurrentState?.FixedUpdate();
    }

    protected virtual void InitializeStates()
    {
        EntityEmptyState = new EntityEmptyState(this);
        EntityHitState = new EntityHitState(this);
        EntityDeathState = new EntityDeathState(this);
    }

    protected void SetStartState(BaseState state)
    {
        CurrentState = state;
        CurrentState.OnEnter();
    }

    protected void SetDefaultState(BaseState state)
    {
        DefaultState = state;
    }

    public void ChangeState(BaseState state)
    {
        if (CurrentState == EntityDeathState) return;
        if (CurrentState == state) return;
        //if (CurrentState.GetType() == state.GetType()) return;

        CurrentState.OnExit();
        CurrentState = state;
        CurrentState.OnEnter();
    }

    protected virtual void CheckGrounded()
    {

    }

    protected virtual void OnDeath()
    {
        ChangeState(EntityDeathState);
    }

    private void HandleHealth()
    {
        CurrentHealth = Mathf.Clamp(CurrentHealth, 0, MaxHealth);
    }

    public void TakeDamage(int dmg, Vector3 hitPoint)
    {
        if (CurrentState == EntityDeathState) return;

        ChangeState(DefaultState);
        ChangeState(EntityHitState);

        HitNumbers hitNumber = Instantiate(hitNumberPrefab, hitPoint, Quaternion.identity);
        hitNumber.ActivateHitNumberText(dmg);

        CurrentHealth -= dmg;

        if(CurrentHealth <= 0 && MaxHealth > 0)
        {
            OnDeath();
        }
    }

    public void Heal(int health)
    {
        CurrentHealth += health;
    }

    public void Kill()
    {
        TakeDamage(int.MaxValue, transform.position);
    }

    public void ChangeTeam(int newTeam)
    {
        Team = newTeam;
    }

    public void SetSpeedModifier(float speed)
    {
        SpeedModifier = speed;
    }

    public void DefaultTransitionToAnimation(string animation)
    {
        animator.CrossFadeInFixedTime(animation, 0.1f);
    }

    public void DefaultTransitionToAnimation(string animation, string layer)
    {
        animator.CrossFadeInFixedTime(animation, 0.1f, animator.GetLayerIndex(layer));
    }

    public void TransitionToAnimation(string animation, float transitionDuration)
    {
        animator.CrossFadeInFixedTime(animation, transitionDuration);
    }

    public void TransitionToAnimation(string animation, float transitionDuration, string layer)
    {
        animator.CrossFadeInFixedTime(animation, transitionDuration, animator.GetLayerIndex(layer));
    }

    public void DestroyEntity()
    {
        Destroy(gameObject);
    }

    public float Distance(Transform transform)
    {
        return Vector3.Distance(transform.position, this.transform.position);
    }

    public float Distance(Entity entity)
    {
        return Vector3.Distance(entity.transform.position, transform.position);
    }

    private protected List<Entity> GetNearbyTargets()
    {
        List<Entity> targets = new List<Entity>();

        Collider[] hits = Physics.OverlapSphere(transform.position, targetDetectionRadius);
        if (hits == null) return targets;
        if (hits.Length == 0) return targets;

        foreach (Collider hit in hits)
        {
            Entity potentialTarget = hit.GetComponent<Entity>();
            if (potentialTarget == null) continue;
            if (potentialTarget.Team == Team) continue;
            targets.Add(potentialTarget);
        }

        return targets.OrderBy(target => Vector3.SqrMagnitude(transform.position - target.transform.position)).ToList();
    }

    private protected List<Entity> GetNearbyTargets(float radius)
    {
        List<Entity> targets = new List<Entity>();

        Collider[] hits = Physics.OverlapSphere(transform.position, radius);
        if (hits == null) return targets;
        if (hits.Length == 0) return targets;

        foreach (Collider hit in hits)
        {
            Entity potentialTarget = hit.GetComponent<Entity>();
            if (potentialTarget == null) continue;
            if (potentialTarget.Team == Team) continue;
            targets.Add(potentialTarget);
        }

        return targets.OrderBy(target => Vector3.SqrMagnitude(transform.position - target.transform.position)).ToList();
    }

    private void IgnoreMyOwnColliders()
    {
        Collider baseCollider = GetComponent<Collider>();
        Collider[] damageableColliders = GetComponentsInChildren<Collider>();
        List<Collider> ignoreColliders = new List<Collider>();

        foreach (Collider collider in damageableColliders)
        {
            ignoreColliders.Add(collider);
        }

        ignoreColliders.Add(baseCollider);

        foreach (Collider c1 in ignoreColliders)
        {
            foreach (Collider c2 in ignoreColliders)
            {
                Physics.IgnoreCollision(c1, c2, true);
            }
        }
    }
}
