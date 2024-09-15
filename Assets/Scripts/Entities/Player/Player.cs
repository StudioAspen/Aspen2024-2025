using KBCore.Refs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Player : Entity
{
    [Header("Player: References")]
    [SerializeField, Self] private InputReader input;
    [SerializeField, Self] private CapsuleCollider capsuleCollider;

    [Header("Player: Capsule Collider Settings")]
    [SerializeField] private float capsuleColliderHeight = 1.8f;
    [Range(0, 1f)]
    [SerializeField] private float stepHeightPercentage = 0.25f;
    private Vector3 startingCapsuleColliderCenter;
    private float startingCapsuleColliderHeight;
    private float startingCapsuleColliderRadius;

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
    [SerializeField] private float jumpToFallDuration = 0.3f;
    [SerializeField] private float inAirDurationToFall = 0.25f;
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
    private float dashTimer;
    private float dashDelayTimer = Mathf.Infinity;
    private Coroutine dashCoroutine;

    [Header("Player: Camera")]
    public bool CameraLocked = true;

    public PlayerIdleState PlayerIdleState { get; private set; }
    public PlayerMoveState PlayerMoveState { get; private set; }
    public PlayerJumpState PlayerJumpState { get; private set; }
    public PlayerDashState PlayerDashState { get; private set; }
    public PlayerSlideState PlayerSlideState { get; private set; }
    public PlayerFallState PlayerFallState { get; private set; }

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
        SaveInitialCapsuleCollider();

        ChangeTeam(0);

        SetStartState(PlayerMoveState);
        SetDefaultState(PlayerMoveState);
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();

        CheckSlopeSliding();

        HandleGrounded();
        HandleFriction();
        HandleFalling();
        HandleDashDelay();
        HandleDashTrail();
        HandleCapsuleCollider();

        HandleAnimations();

        Cursor.lockState = CameraLocked ? CursorLockMode.Locked : CursorLockMode.None;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, transform.position + CurrentMovementSpeed * targetForwardDirection);
        Gizmos.DrawWireSphere(transform.position + 9f * controller.radius / 10f * Vector3.up, controller.radius);
    }

    protected override void InitializeStates()
    {
        base.InitializeStates();

        PlayerIdleState = new PlayerIdleState(this, 0);
        PlayerMoveState = new PlayerMoveState(this, 0);
        PlayerJumpState = new PlayerJumpState(this, jumpToFallDuration, 0);
        PlayerDashState = new PlayerDashState(this, 0);
        PlayerSlideState = new PlayerSlideState(this, 0);
        PlayerFallState = new PlayerFallState(this, 0);
    }

    private void HandleJumpInput()
    {
        if (!IsGrounded && currentJumpCount >= maxJumpCount) return;
        if (currentState == PlayerSlideState) return;

        ChangeState(PlayerJumpState, false);
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
                ChangeState(PlayerDashState, false);
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
        controller.Move(Time.deltaTime * groundedVelocity);
    }

    public void HandleVelocity()
    {
        if (groundedVelocity.sqrMagnitude < Mathf.Pow(CurrentMovementSpeed, 2))
        {
            velocity += groundedAcceleration * Time.deltaTime * targetForwardDirection;
        }
    }

    public void HandleFriction()
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

    private void HandleFalling()
    {
        if(inAirTimer > inAirDurationToFall)
        {
            ChangeState(PlayerFallState, false);
        }
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
        if (!IsGrounded)
        {
            hitBelowSlopeAngle = 0f;
            return;
        }

        Physics.Raycast(transform.position, Vector3.down, out hitBelow, controller.height / 2f, groundLayer);

        if (hitBelow.collider == null)
        {
            hitBelowSlopeAngle = 0f;
            return;
        }

        Vector3 normal = hitBelow.normal;

        hitBelowSlopeAngle = Vector3.Angle(normal, Vector3.up);

        if (IsAbleToSlide())
        {
            Vector3 slideDirection = Vector3.ProjectOnPlane(Vector3.down, normal);

            PlayerSlideState.SetSlideDirection(slideDirection);
            ChangeState(PlayerSlideState, false);
        }
        else
        {
            hitBelowSlopeAngle = 0f;
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
        CurrentMovementSpeed = Mathf.Lerp(CurrentMovementSpeed, 0f, groundedAcceleration * Time.deltaTime);
    }

    public void SetMovingSpeed()
    {
        CurrentMovementSpeed = IsSprinting ? Mathf.Lerp(CurrentMovementSpeed, maxSpeed, groundedAcceleration * Time.deltaTime) : Mathf.Lerp(CurrentMovementSpeed, walkSpeed, groundedAcceleration * Time.deltaTime);
    }

    private void HandleAnimations()
    {
        animator.SetFloat("MovementSpeed", CurrentMovementSpeed/maxSpeed);
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

        CurrentMovementSpeed = initialDashVelocity;
        for (float t = 0; t < dashDuration; t += Time.deltaTime)
        {
            CurrentMovementSpeed = (initialDashVelocity - maxSpeed) * (1 - Mathf.Sqrt(1 - Mathf.Pow(t / dashDuration - 1, 2))) + maxSpeed;

            transform.rotation = Quaternion.Slerp(transform.rotation, targetForwardRotation, rotationSpeed * Time.deltaTime);
            controller.Move(CurrentMovementSpeed * Time.deltaTime * targetForwardDirection);
            yield return null;
        }
        CurrentMovementSpeed = maxSpeed;

        ChangeState(PlayerMoveState, false);

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
        dashTrailObject.SetActive(CurrentMovementSpeed > maxSpeed);
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

    private void SaveInitialCapsuleCollider()
    {
        startingCapsuleColliderCenter = capsuleCollider.center;
        startingCapsuleColliderHeight = capsuleCollider.height;
        startingCapsuleColliderRadius = capsuleCollider.radius;
    }

    private void HandleCapsuleCollider()
    {
        capsuleColliderHeight = Mathf.Clamp(capsuleColliderHeight, 0f, Mathf.Infinity);

        float colliderHeightDiff = startingCapsuleColliderHeight - capsuleColliderHeight;

        Vector3 newColliderCenter = new Vector3(0f, startingCapsuleColliderCenter.y + (colliderHeightDiff / 2f), 0f);

        capsuleCollider.center = newColliderCenter;
        capsuleCollider.height = capsuleColliderHeight;

        float halfColliderHeight = capsuleColliderHeight / 2f;
        capsuleCollider.radius = halfColliderHeight < startingCapsuleColliderRadius ? halfColliderHeight : startingCapsuleColliderRadius;
    }
}
