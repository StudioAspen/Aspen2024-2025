using KBCore.Refs;
using UnityEngine;

public class Entity : MonoBehaviour
{
    [Header("Entity: References")]
    [SerializeField, Self] private protected Rigidbody rigidBody;
    [SerializeField, Self] private protected Animator animator;
    [SerializeField] private protected GlobalPhysicsSettings physicsSettings;

    [field: Header("Entity: Settings")]
    [field: SerializeField] public int CurrentHealth { get; private set; }
    [field: SerializeField] public int MaxHealth { get; private set; }
    [field: SerializeField] public int Level { get; private set; }

    [HideInInspector] public bool IsGrounded;

    [SerializeField] protected private LayerMask groundLayer;
    public float CurrentMovementSpeed { get; protected set; }
    [SerializeField] protected private Vector3 velocity;
    protected private Vector3 groundedVelocity => new Vector3(velocity.x, 0f, velocity.z);
    protected private float inAirTimer;
    protected private bool fallVelocityApplied;

    public int Team { get; private set; }

    protected private BaseState currentState;
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
        currentState?.Update();

        CheckGrounded();
    }

    private void FixedUpdate()
    {
        OnFixedUpdate();
    }

    protected virtual void OnFixedUpdate()
    {
        currentState?.FixedUpdate();
    }

    protected virtual void InitializeStates()
    {

    }

    protected void SetStartState(BaseState state)
    {
        currentState = state;
        currentState.OnEnter();
    }

    protected void SetDefaultState(BaseState state)
    {
        DefaultState = state;
    }

    public void ChangeState(BaseState newState, bool overridePrio)
    {
        if (currentState == newState) return;
        if (currentState.Priority > newState.Priority && !overridePrio) return;

        currentState.OnExit();
        currentState = newState;
        currentState.OnEnter();
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

    protected virtual void CheckGrounded()
    {
        if (inAirTimer > 0f && inAirTimer < 0.1f)
        {
            IsGrounded = false;
            return;
        }
    }
}
