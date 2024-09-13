using KBCore.Refs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Player : Entity
{
    [Header("Player: References")]
    [SerializeField, Self] private CharacterController controller;
    [SerializeField, Self] private InputReader input;

    [Header("Player: Grounded Movement")]
    [SerializeField] private float walkSpeed = 3f;
    [SerializeField] private float maxSpeed = 5f;
    [SerializeField] private float rotationSpeed = 5f;
    public Vector3 MoveDirection => input.MoveDirection;
    private float forwardAngleBasedOnCamera;
    private Quaternion targetForwardRotation = Quaternion.identity;
    private Vector3 targetForwardDirection = Vector3.forward;
    private RaycastHit hitBelow;
    private float hitBelowSlopeAngle;

    [Header("Player: Gravity")]
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private int maxJumpCount = 1;
    [SerializeField] private float groundedAcceleration = 4f;
    private int currentJumpCount;
    
    #region Flags
    [HideInInspector] public bool IsMoving => input.MoveDirection.sqrMagnitude > 0;
    [HideInInspector] public bool IsSprinting;
    [HideInInspector] public bool IsAttacking;
    [HideInInspector] public bool IsChargingAttack;
    [HideInInspector] public bool CanAttack = true;
    [HideInInspector] public bool IsJumping;
    #endregion

    [Header("Player: Dash")]
    [SerializeField] private float dashDuration = 0.5f;
    [SerializeField] private float initialDashVelocity = 25f;
    [SerializeField] private float dashDelayDuration = 0.5f;
    [SerializeField] private float sprintDurationAfterDash = 2f;
    [SerializeField] private float shiftKeyPressMaxDurationForDash = 0.25f;
    [SerializeField] private GameObject dashTrailObject;
    private float shiftKeyPressTimer;
    private float dashDelayTimer = Mathf.Infinity;
    private Coroutine dashCoroutine;

    [Header("Player: Camera")]
    public bool CameraLocked = true;

    public PlayerIdleState PlayerIdleState { get; private set; }
    public PlayerMoveState PlayerMoveState { get; private set; }
    public PlayerJumpState PlayerJumpState { get; private set; }
    public PlayerDashState PlayerDashState { get; private set; }
    public PlayerSlideState PlayerSlideState { get; private set; }

    private void OnEnable()
    {
        input.Jump.AddListener(HandleJumpInput);
        input.SprintHold.AddListener(HandleSprintInput);
        input.SprintRelease.AddListener(HandleDashInput);
    }

    private void OnDisable()
    {
        input.Jump.RemoveListener(HandleJumpInput);
        input.SprintHold.RemoveListener(HandleSprintInput);
        input.SprintRelease.RemoveListener(HandleDashInput);
    }

    protected override void OnAwake()
    {
        base.OnAwake();
    }

    protected override void OnStart()
    {
        base.OnStart();

        IgnoreMyOwnColliders();

        ChangeTeam(0);

        SetStartState(PlayerIdleState);
        SetDefaultState(PlayerIdleState);
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();

        CheckGrounded();
        CheckSlopeSliding();

        HandleGravity();
        HandleGrounded();
        HandleDashDelay();
        HandleDashTrail();

        HandleAnimations();

        Cursor.lockState = CameraLocked ? CursorLockMode.Locked : CursorLockMode.None;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, transform.position + currentMovementSpeed * targetForwardDirection);
        Gizmos.DrawWireSphere(transform.position + 9f * controller.radius / 10f * Vector3.up, controller.radius);
    }

    protected override void InitializeStates()
    {
        base.InitializeStates();

        PlayerIdleState = new PlayerIdleState(this);
        PlayerMoveState = new PlayerMoveState(this);
        PlayerJumpState = new PlayerJumpState(this);
        PlayerDashState = new PlayerDashState(this);
        PlayerSlideState = new PlayerSlideState(this);
    }

    private void CheckGrounded()
    {
        if (inAirTimer > 0f && inAirTimer < 0.1f)
        {
            IsGrounded = false;
            return;
        }

        IsGrounded = Physics.CheckSphere(transform.position + 9f * controller.radius / 10f * Vector3.up, controller.radius, groundLayer);
    }

    private void HandleJumpInput()
    {
        if (!IsGrounded && currentJumpCount >= maxJumpCount) return;
        if (currentState == PlayerSlideState) return;

        ChangeState(PlayerJumpState);
    }

    private void HandleSprintInput()
    {
        IsSprinting = true;
        shiftKeyPressTimer += Time.unscaledDeltaTime;
    }

    private void HandleDashInput()
    {
        if (dashDelayTimer > dashDelayDuration)
        {
            if (shiftKeyPressTimer < shiftKeyPressMaxDurationForDash)
            {
                ChangeState(PlayerDashState);
            }
            else
            {
                IsSprinting = false;
            }
        }
        shiftKeyPressTimer = 0f;
    }

    public void HandleGroundedMovement()
    {
        Vector3 groundedVelocity = new Vector3(velocity.x, 0f, velocity.z);
        groundedVelocity = Vector3.ClampMagnitude(groundedVelocity, currentMovementSpeed);

        controller.Move(groundedVelocity * Time.deltaTime);
    }

    public void HandleMovingVelocity()
    {
        velocity.x = Mathf.Lerp(velocity.x, currentMovementSpeed * targetForwardDirection.x, groundedAcceleration * Time.deltaTime);
        velocity.z = Mathf.Lerp(velocity.z, currentMovementSpeed * targetForwardDirection.z, groundedAcceleration * Time.deltaTime);     
    }

    public void HandleIdleVelocity()
    {
        velocity.x = Mathf.Lerp(velocity.x, 0f, groundedAcceleration * Time.deltaTime);
        velocity.z = Mathf.Lerp(velocity.z, 0f, groundedAcceleration * Time.deltaTime);
    }

    private void HandleGrounded()
    {
        if (IsGrounded)
        {
            if (currentState != PlayerSlideState)
            {
                currentJumpCount = 0;
            }
            inAirTimer = 0f;
            fallVelocityApplied = false;
            IsJumping = false;
        }
        else // in air
        {
            if (!IsJumping && !fallVelocityApplied) // falling without jumping
            {
                fallVelocityApplied = true;
                velocity.y = physicsSettings.FallingStartingYVelocity;
            }
            inAirTimer += Time.deltaTime;
            velocity.y += physicsSettings.Gravity * Time.deltaTime;
        }
    }

    private void HandleGravity()
    {
        controller.Move(Time.deltaTime * velocity.y * Vector3.up);
    }

    public void HandleRotation()
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, targetForwardRotation, rotationSpeed * Time.deltaTime);
    }

    private void HandleDashDelay()
    {
        dashDelayTimer += Time.deltaTime;
    }

    public void ResetDashDelay()
    {
        dashDelayTimer = 0f;
    }

    private void CheckSlopeSliding()
    {
        if (!IsGrounded) return;

        Physics.SphereCast(transform.position + controller.height/2 * Vector3.up, controller.radius, Vector3.down, out hitBelow, controller.height, groundLayer);

        if (hitBelow.collider == null) return;

        Vector3 normal = hitBelow.normal;

        hitBelowSlopeAngle = Vector3.Angle(normal, Vector3.up);

        if (IsAbleToSlide())
        {
            Vector3 slideDirection = Vector3.ProjectOnPlane(Vector3.down, normal);

            PlayerSlideState.SetSlideDirection(slideDirection);
            ChangeState(PlayerSlideState);
        }
    }

    public bool IsAbleToSlide()
    {
        if (!IsGrounded) return false;
        if (hitBelow.collider == null) return false;

        return hitBelowSlopeAngle > controller.slopeLimit;
    }

    public void ApplySlide(Vector3 slideDirection)
    {
        velocity.y = physicsSettings.GroundedYVelocity;
        controller.Move(slideDirection * -velocity.y * Time.deltaTime);
    }

    public void ApplyRotationToNextMovement()
    {
        forwardAngleBasedOnCamera = Mathf.Atan2(input.MoveDirection.x, input.MoveDirection.z) * Mathf.Rad2Deg + Camera.main.transform.rotation.eulerAngles.y;
        targetForwardRotation = Quaternion.Euler(0, forwardAngleBasedOnCamera, 0);
        targetForwardDirection = targetForwardRotation * Vector3.forward;
    }

    public void SetIdleSpeed()
    {
        currentMovementSpeed = Mathf.Lerp(currentMovementSpeed, 0f, 10f * Time.deltaTime);
    }

    public void SetMovingSpeed()
    {
        currentMovementSpeed = IsSprinting ? Mathf.Lerp(currentMovementSpeed, maxSpeed, 10f * Time.deltaTime) : Mathf.Lerp(currentMovementSpeed, walkSpeed, 10f * Time.deltaTime);
    }

    private void HandleAnimations()
    {
        animator.SetFloat("MovementSpeed", currentMovementSpeed/maxSpeed);
        animator.SetFloat("InAirTimer", inAirTimer);
        animator.SetBool("IsGrounded", IsGrounded);
    }

    public void Jump()
    {
        input.OnPlayerActionInput?.Invoke(PlayerActions.Jump);

        IsJumping = true;
        IsGrounded = false;

        velocity.y = Mathf.Sqrt(jumpHeight * -2f * physicsSettings.Gravity);
        inAirTimer = 0.01f;

        animator.CrossFadeInFixedTime("JumpingUp", 0.1f);

        currentJumpCount++;
    }

    public void Dash()
    {
        input.OnPlayerActionInput?.Invoke(PlayerActions.Dash);

        StopDashing();
        dashCoroutine = StartCoroutine(DashCoroutine());
        dashDelayTimer = 0f;
    }

    public IEnumerator DashCoroutine()
    {
        if (IsGrounded) animator.CrossFadeInFixedTime("Dash", 0.1f);

        float currDashVelocity = initialDashVelocity;
        for (float t = 0; t < dashDuration; t += Time.deltaTime)
        {
            currDashVelocity = (initialDashVelocity - maxSpeed) * (1 - Mathf.Sqrt(1 - Mathf.Pow(t / dashDuration - 1, 2))) + maxSpeed;

            velocity.x = currDashVelocity * targetForwardDirection.x;
            velocity.z = currDashVelocity * targetForwardDirection.z;

            Vector3 groundedVelocity = new Vector3(velocity.x, 0f, velocity.z);
            groundedVelocity = Vector3.ClampMagnitude(groundedVelocity, currDashVelocity);

            transform.rotation = Quaternion.Slerp(transform.rotation, targetForwardRotation, rotationSpeed * Time.deltaTime);
            controller.Move(groundedVelocity * Time.deltaTime);
            yield return null;
        }

        ChangeState(PlayerMoveState);

        StartCoroutine(SprintAfterDashCoroutine());
    }

    private IEnumerator SprintAfterDashCoroutine()
    {
        IsSprinting = true;

        dashDelayTimer = 0f;

        if (IsGrounded) animator.CrossFadeInFixedTime("FlatMovement", 0.1f);

        for (float t = 0; t < sprintDurationAfterDash; t += Time.deltaTime)
        {
            if (!IsMoving) break;
            yield return null;
        }

        IsSprinting = false;
    }

    public void StopDashing()
    {
        if (dashCoroutine == null) return;
            
        StopCoroutine(dashCoroutine);
    }

    public void HandleDashTrail()
    {
        Vector3 groundedVelocity = new Vector3(velocity.x, 0f, velocity.z);
        dashTrailObject.SetActive(groundedVelocity.magnitude > maxSpeed);
    }

    private void IgnoreMyOwnColliders()
    {
        Collider baseCollider = GetComponent<Collider>();
        Collider[] damageableColliders = GetComponentsInChildren<Collider>();
        List<Collider> ignoreColliders = new List<Collider>();

        foreach(Collider collider in damageableColliders)
        {
            ignoreColliders.Add(collider);
        }

        ignoreColliders.Add(baseCollider);

        foreach(Collider c1 in ignoreColliders)
        {
            foreach(Collider c2 in ignoreColliders)
            {
                Physics.IgnoreCollision(c1, c2, true);
            }
        }
    }
}
