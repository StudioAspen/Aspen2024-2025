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
    public float MovementSpeed => movementOnSlopeSpeedModifier * SpeedModifier * baseSpeed;
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
    public PlayerAttackState PlayerAttackState { get; private set; }
    public PlayerChargeState PlayerChargeState { get; private set; }

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

        HandleGrounded();
        HandleAirborne();
        HandleDashDelay();
        HandleDashTrail();

        HandleAnimations();

        Cursor.lockState = CameraLocked ? CursorLockMode.Locked : CursorLockMode.None;

        stateText.text = $"State: {CurrentState.GetType().ToString()}";
    }

    private void OnAnimatorMove()
    {
        if (CurrentState != PlayerAttackState) return;

        if (!ApplyRootMotion) return;

        Vector3 desiredAnimationMovement = animator.deltaPosition;
        //desiredAnimationMovement.y = 0f;

        controller.Move(desiredAnimationMovement);
    }

    protected override void InitializeStates()
    {
        base.InitializeStates();

        PlayerIdleState = new PlayerIdleState(this);
        PlayerWalkingState = new PlayerWalkingState(this);
        PlayerSprintingState = new PlayerSprintingState(this);
        PlayerJumpState = new PlayerJumpState(this);
        PlayerFallState = new PlayerFallState(this);
        PlayerDashState = new PlayerDashState(this);
        PlayerSlideState = new PlayerSlideState(this);
        PlayerAttackState = new PlayerAttackState(this);
        PlayerChargeState = new PlayerChargeState(this);
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
        if(CurrentState == PlayerChargeState) return;
        if (CurrentState == PlayerAttackState) return;


        ChangeState(PlayerJumpState);
    }

    private void HandleSprintInput()
    {
        if (CurrentState == PlayerChargeState) return;

        IsSprinting = true;
        shiftKeyPressTimer += Time.unscaledDeltaTime;
    }

    private void HandleDashInput()
    {
        if (dashDelayTimer > dashDelayDuration)
        {
            if (shiftKeyPressTimer < shiftKeyPressMaxDurationForDash)
            {
                if (CurrentState != PlayerChargeState) ChangeState(PlayerDashState);
                IsSprinting = false;
            }
            else
            {
                IsSprinting = false;
            }
        }
        shiftKeyPressTimer = 0f;
    }

    public void GroundedMove()
    {
        controller.Move(GetGroundedVelocity() * Time.deltaTime);
    }

    public void AccelerateToSpeed(float speed)
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

    private void HandleGrounded()
    {
        if (IsGrounded)
        {
            if (CurrentState != PlayerSlideState)
            {
                currentJumpCount = 0;
            }
            inAirTimer = 0f;
            fallVelocityApplied = false;
            IsJumping = false;
        }
    }

    public void HandleAirborne()
    {
        if (!IsGrounded)
        {
            if (!IsJumping && !fallVelocityApplied) // falling without jumping
            {
                fallVelocityApplied = true;
                velocity.y = physicsSettings.FallingStartingYVelocity;

                if (CurrentState != PlayerAttackState && CurrentState != PlayerDashState) ChangeState(PlayerFallState);
            }
            inAirTimer += Time.deltaTime;
            velocity.y += physicsSettings.Gravity * Time.deltaTime;
        }
    }

    public void ApplyGravity()
    {
        controller.Move(Time.deltaTime * velocity.y * Vector3.up);
    }

    public void ResetYVelocity()
    {
        velocity.y = 0f;
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

    private void CheckSlopeSliding()
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
            ChangeState(PlayerSlideState);
        }
    }

    public bool IsAbleToSlide()
    {
        if (!IsGrounded) return false;
        if (hitBelow.collider == null) return false;
        if (CurrentState == PlayerAttackState) return false;

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
        totalSpeedModifierForAnimation = Mathf.Lerp(totalSpeedModifierForAnimation, movementOnSlopeSpeedModifier * SpeedModifier, groundedAcceleration * Time.deltaTime); 

        animator.SetFloat("MovementSpeed", totalSpeedModifierForAnimation);
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

    public float Distance(Vector3 pos)
    {
        return Vector3.Distance(transform.position, pos);
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
}
