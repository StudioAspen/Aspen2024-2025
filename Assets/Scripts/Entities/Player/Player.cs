using KBCore.Refs;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Player : Entity
{
    [Header("Player: Debug UI")]
    [SerializeField] private TMP_Text stateText;

    [Header("Player: References")]
    [SerializeField, Self] private CharacterController controller;
    [SerializeField, Self] private InputReader input;

    [Header("Player: Grounded Movement")]
    [SerializeField] private float rotationSpeed = 5f;
    [field: SerializeField] public float SprintSpeedModifier { get; private set; } = 1.66f;
    public float MovementSpeed => movementOnSlopeSpeedModifier * speedModifier * baseSpeed;
    private float movementOnSlopeSpeedModifier = 1f;
    private float totalSpeedModifierForAnimation;
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
    [field: SerializeField] public float JumpTimeToFall { get; private set; } = 0.3f;
    private int currentJumpCount;
    
    #region Flags
    [HideInInspector] public bool IsMoving => input.MoveDirection.sqrMagnitude > 0;
    [HideInInspector] public bool IsSprinting;
    [HideInInspector] public bool IsAttacking;
    [HideInInspector] public bool IsChargingAttack;
    [HideInInspector] public bool CanAttack = true;
    [HideInInspector] public bool IsJumping;
    [HideInInspector] public bool ApplyRootMotion;
    #endregion

    [field : Header("Player: Dash")]
    [field: SerializeField] public float DashDuration { get; private set; } = 0.5f;
    [field : SerializeField] public float InitialDashVelocity { get; private set; } = 25f;
    [SerializeField] private float dashDelayDuration = 0.5f;
    [field: SerializeField] public float SprintDurationAfterDash { get; private set; } = 2f;
    [SerializeField] private float shiftKeyPressMaxDurationForDash = 0.25f;
    [SerializeField] private GameObject dashTrailObject;
    private float shiftKeyPressTimer;
    private float dashDelayTimer = Mathf.Infinity;
    private Coroutine dashCoroutine;

    [Header("Player: Camera")]
    public bool CameraLocked = true;

    public PlayerIdleState PlayerIdleState { get; private set; }
    public PlayerWalkingState PlayerWalkingState { get; private set; }
    public PlayerSprintingState PlayerSprintingState { get; private set; }
    public PlayerJumpState PlayerJumpState { get; private set; }
    public PlayerFallState PlayerFallState { get; private set; }
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

        HandleDashDelay();
        HandleDashTrail();

        HandleAnimations();

        Cursor.lockState = CameraLocked ? CursorLockMode.Locked : CursorLockMode.None;

        stateText.text = $"State: {CurrentState.GetType().ToString()}";
    }

    private void OnAnimatorMove()
    {
        if (!IsAttacking) return;

        if (!ApplyRootMotion) return;

        Vector3 desiredAnimationMovement = animator.deltaPosition;
        //desiredAnimationMovement.y = 0f;

        controller.Move(desiredAnimationMovement);
    }

    protected override void InitializeStates()
    {
        base.InitializeStates();

        PlayerIdleState = BaseState.CreateState<PlayerIdleState>(this, 0);
        PlayerWalkingState = BaseState.CreateState<PlayerWalkingState>(this, 0);
        PlayerSprintingState = BaseState.CreateState<PlayerSprintingState>(this, 0);
        PlayerJumpState = BaseState.CreateState<PlayerJumpState>(this, 0);
        PlayerFallState = BaseState.CreateState<PlayerFallState>(this, 0);
        PlayerDashState = BaseState.CreateState<PlayerDashState>(this, 1);
        PlayerSlideState = BaseState.CreateState<PlayerSlideState>(this, 2);
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
        if (CurrentState == PlayerSlideState) return;

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

    public void HandleGrounded()
    {
        if (CurrentState != PlayerSlideState)
        {
            currentJumpCount = 0;
        }
        inAirTimer = 0f;
        fallVelocityApplied = false;
        IsJumping = false;
    }

    public bool IsFalling()
    {
        return !IsGrounded && !IsJumping && !fallVelocityApplied;
    }

    public void HandleAirborne()
    {
        if (!IsJumping && !fallVelocityApplied) // falling without jumping
        {
            ChangeState(PlayerFallState, false);
        }
        inAirTimer += Time.deltaTime;
    }

    public void StartFall()
    {
        fallVelocityApplied = true;
        velocity.y = physicsSettings.FallingStartingYVelocity;
    }

    public void GroundedMove()
    {
        controller.Move(GetGroundedVelocity() * Time.deltaTime);
    }

    public void AccelerateToTargetSpeed(float speed)
    {
        Vector3 groundedVelocity = GetGroundedVelocity();

        groundedVelocity = Vector3.Lerp(groundedVelocity, speed * targetForwardDirection, groundedAcceleration * Time.deltaTime);

        velocity.x = groundedVelocity.x;
        velocity.z = groundedVelocity.z;
    }

    public void InstantlySetSpeed(float speed)
    {
        Vector3 groundedVelocity = GetGroundedVelocity();

        groundedVelocity = speed * targetForwardDirection;

        velocity.x = groundedVelocity.x;
        velocity.z = groundedVelocity.z;
    }

    public void ApplyGravity()
    {
        if(!IsGrounded) velocity.y += physicsSettings.Gravity * Time.deltaTime;

        controller.Move(Time.deltaTime * velocity.y * Vector3.up);
    }

    public void RotateToTargetRotation()
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, targetForwardRotation, rotationSpeed * Time.deltaTime);
    }

    private void HandleDashDelay()
    {
        dashDelayTimer += Time.deltaTime;
    }

    public void ResetDashDelay()
    {
        dashDelayTimer = 0f;
    }

    public void CheckSlopeSliding()
    {
        GetAndSetSlopeSpeedModifierOnAngle(hitBelowSlopeAngle);

        if (!IsGrounded)
        {
            hitBelowSlopeAngle = 0f;
            return;
        }

        Physics.Raycast(transform.position, Vector3.down, out hitBelow, controller.height / 2, groundLayer, QueryTriggerInteraction.Ignore);

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

    private void HandleAnimations()
    {
        totalSpeedModifierForAnimation = Mathf.Lerp(totalSpeedModifierForAnimation, movementOnSlopeSpeedModifier * speedModifier, groundedAcceleration * Time.deltaTime); 

        animator.SetFloat("MovementSpeed", totalSpeedModifierForAnimation);
    }

    public void Jump()
    {
        input.OnPlayerActionInput?.Invoke(PlayerActions.Jump);

        IsJumping = true;
        IsGrounded = false;

        velocity.y = Mathf.Sqrt(jumpHeight * -2f * physicsSettings.Gravity);
        inAirTimer += Time.deltaTime; // increase air timer to disable immediate grounding

        currentJumpCount++;
    }

    public void Dash()
    {
        input.OnPlayerActionInput?.Invoke(PlayerActions.Dash);
        dashDelayTimer = 0f;
    }

    public void DashTrailSetActive(bool b)
    {
        dashTrailObject.SetActive(b);
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

    private float GetAndSetSlopeSpeedModifierOnAngle(float groundAngle)
    {
        float slopeSpeedModifier = 1f - (0.15f) * groundAngle / controller.slopeLimit;

        if (groundAngle > controller.slopeLimit) slopeSpeedModifier = 0.85f;

        movementOnSlopeSpeedModifier = slopeSpeedModifier;

        return slopeSpeedModifier;
    }

    public Vector3 GetGroundedVelocity()
    {
        return new Vector3(velocity.x, 0f, velocity.z);
    }

    public float GetMaxSpeed()
    {
        return SprintSpeedModifier * baseSpeed;
    }

    public void DefaultTransitionToAnimation(string animation)
    {
        animator.CrossFadeInFixedTime(animation, 0.1f);
    }

    public void TransitionToAnimation(string animation, float transitionDuration)
    {
        animator.CrossFadeInFixedTime(animation, transitionDuration);
    }

    private void HandleDashTrail()
    {
        float maxSpeed = SprintSpeedModifier * baseSpeed;

        DashTrailSetActive(GetGroundedVelocity().magnitude > maxSpeed);
    }

    public void ReplaceComboAnimationClip(AnimationClip newClip)
    {
        AnimatorOverrideController aoc = new AnimatorOverrideController(animator.runtimeAnimatorController);

        var anims = new List<KeyValuePair<AnimationClip, AnimationClip>>();

        foreach (AnimationClip currentClip in aoc.animationClips)
        {
            if (currentClip.name == "ComboPlaceholder")
            {
                anims.Add(new KeyValuePair<AnimationClip, AnimationClip>(currentClip, newClip));
            }
        }

        aoc.ApplyOverrides(anims);

        animator.runtimeAnimatorController = aoc;
    }

    public void SetComboAnimationSpeed(float speed)
    {
        animator.SetFloat("ComboAnimationSpeed", speed);
    }

    public void ResetYVelocity()
    {
        velocity.y = 0f;
    }
}
