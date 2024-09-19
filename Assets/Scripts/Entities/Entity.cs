using KBCore.Refs;
using UnityEngine;

public class Entity : MonoBehaviour
{
    [Header("Entity: References")]
    [SerializeField, Self] private protected Animator animator;
    [SerializeField] private protected GlobalPhysicsSettings physicsSettings;

    [field: Header("Entity: Settings")]
    [field: SerializeField] public int CurrentHealth { get; private set; }
    [field: SerializeField] public int MaxHealth { get; private set; }
    [field: SerializeField] public int Level { get; private set; }

    [HideInInspector] public bool IsGrounded;

    [SerializeField] protected float baseSpeed = 3f;
    protected float speedModifier = 1f;
    [SerializeField] protected private Vector3 velocity;
    [SerializeField] protected private LayerMask groundLayer;
    protected private float inAirTimer;
    protected private bool fallVelocityApplied;

    public int Team { get; private set; }

    public BaseState CurrentState { get; private set; }
    public BaseState DefaultState { get; private set; }

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
        CurrentHealth = MaxHealth;
    }

    private void Update()
    {
        OnUpdate();   
    }

    protected virtual void OnUpdate()
    {
        CurrentState?.Update();
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
        if (CurrentState == state) return;

        CurrentState.OnExit();
        CurrentState = state;
        CurrentState.OnEnter();
    }

    protected virtual void Die()
    {
        OnDeath();
    }

    protected virtual void OnDeath()
    {
        
    }

    private void HandleHealth()
    {
        CurrentHealth = Mathf.Clamp(CurrentHealth, 0, MaxHealth);
    }

    public void TakeDamage(int dmg)
    {
        CurrentHealth -= dmg;

        if(CurrentHealth <= 0 && MaxHealth > 0)
        {
            Die();
        }
    }

    public void Heal(int health)
    {
        CurrentHealth += health;
    }

    public void Kill()
    {
        TakeDamage(int.MaxValue);
    }

    public void ChangeTeam(int newTeam)
    {
        Team = newTeam;
    }

    public void SetSpeedModifier(float speed)
    {
        speedModifier = speed;
    }
}
