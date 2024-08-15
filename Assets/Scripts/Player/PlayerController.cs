using KBCore.Refs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField, Self] private CharacterController controller;
    [SerializeField, Self] private InputReader input;
    [SerializeField, Self] private Animator animator;

    [Header("Player Grounded Movement")]
    [SerializeField] private float walkSpeed = 3f;
    [SerializeField] private float maxSpeed = 5f;
    [SerializeField] private float rotationSpeed = 5f;
    private float currentMovementSpeed;
    private float forwardAngleBasedOnCamera;
    private Quaternion targetForwardRotation = Quaternion.identity;
    private Vector3 targetForwardDirection = Vector3.forward;

    [Header("Player Gravity")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private int maxJumpCount = 2;
    [SerializeField] private Vector3 acceleration = new Vector3(4f, -20f, 4f);
    [SerializeField] private Vector3 velocity;
    [SerializeField] private float groundedYVelocity = -5f;
    [SerializeField] private float fallingStartingYVelocity = 0f;
    private float inAirTimer = 0;
    private int currentJumpCount;
    private bool fallVelocityApplied;

    #region Flags
    [HideInInspector] public bool IsGrounded;
    [HideInInspector] public bool CanMove = true;
    [HideInInspector] public bool IsMoving;
    [HideInInspector] public bool IsSprinting;
    [HideInInspector] public bool IsDashing;
    [HideInInspector] public bool IsAttacking;
    [HideInInspector] public bool CanAttack = true;
    [HideInInspector] public bool IsJumping;
    [HideInInspector] public bool IsUsingSkill;
    #endregion

    [Header("Combat")]
    [SerializeField] private WeaponHandler weapon;
    private float instantaneousAttackAngle;
    [SerializeField] private float maxComboDelay = 0.5f;
    [SerializeField] private float attackAnimationSpeedMultiplier = 3f;
    private int comboIndex;
    private Coroutine currentSwingingCoroutine;

    [Header("Dash")]
    [SerializeField] private float dashDuration = 0.5f;
    [SerializeField] private float initialDashVelocity = 25f;
    [SerializeField] private float dashDelayDuration = 0.5f;
    [SerializeField] private float sprintDurationAfterDash = 2f;
    [SerializeField] private float shiftKeyPressMaxDurationForDash = 0.1f;
    [SerializeField] private GameObject dashTrailObject;
    private float shiftKeyPressTimer;
    private float dashDelayTimer = Mathf.Infinity;
    private Coroutine dashCoroutine;

    [Header("Camera")]
    public bool CameraLocked = true;

    private void OnValidate()
    {
        this.ValidateRefs();
    }

    private void Awake()
    {
        
    }

    void Start()
    {
        IgnoreMyOwnColliders();

        dashTrailObject.SetActive(false);
    }

    void Update()
    {
        CheckGrounded();
        
        HandleGroundedMovementInput();
        HandleJumpInput();
        HandleSprintInput();
        HandleDashInput();
        HandleSwingInput();

        HandleGroundedMovement();
        HandleGravity();
        HandleSpeed();

        HandleWeaponCollisions();

        HandleAnimations();

        Cursor.lockState = CameraLocked ? CursorLockMode.Locked : CursorLockMode.None;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, transform.position + currentMovementSpeed * targetForwardDirection);
    }

    private void CheckGrounded()
    {
        IsGrounded = Physics.CheckSphere(transform.position + controller.radius / 2 * Vector3.up, controller.radius, groundLayer);
    }

    private void HandleGroundedMovementInput()
    {
        IsMoving = input.MoveDirection.magnitude > 0;

        if (!IsMoving) return;

        forwardAngleBasedOnCamera = Mathf.Atan2(input.MoveDirection.x, input.MoveDirection.z) * Mathf.Rad2Deg + Camera.main.transform.rotation.eulerAngles.y;
        targetForwardRotation = Quaternion.Euler(0, forwardAngleBasedOnCamera, 0);
        targetForwardDirection = targetForwardRotation * Vector3.forward;
    }

    private void HandleJumpInput()
    {
        if (!CanMove) return;
        if (!IsGrounded && currentJumpCount >= maxJumpCount) return;

        if (input.Jump)
        {
            StopCurrentSwingCoroutine();

            IsJumping = true;
            IsGrounded = false;

            velocity.y = Mathf.Sqrt(jumpHeight * -2f * acceleration.y);

            animator.CrossFadeInFixedTime("JumpingUp", 0.1f);

            currentJumpCount++;
        }
    }

    private void HandleSprintInput()
    {
        if (!CanMove) return;

        if (input.SprintHold)
        {
            IsSprinting = true;
            shiftKeyPressTimer += Time.unscaledDeltaTime;
        }
    }

    private void HandleDashInput()
    {
        if(!IsDashing) dashDelayTimer += Time.deltaTime;

        if (input.SprintRelease)
        {
            if (dashDelayTimer > dashDelayDuration)
            {
                if (shiftKeyPressTimer < shiftKeyPressMaxDurationForDash)
                {
                    Dash();
                }
                else
                {
                    IsSprinting = false;
                }
            }
            shiftKeyPressTimer = 0f;
        }
    }

    private void HandleSwingInput()
    {
        if (input.Attack)
        {
            if (!CanAttack) return;
            if (IsAttacking) return;

            SwingMeleeWeapon(weapon.Combo.PrimaryCombo[comboIndex].AnimationClipName);
        }
    }

    private void HandleGroundedMovement()
    {
        if (!CanMove) return;
        if (IsDashing) return;

        if (IsMoving)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetForwardRotation, rotationSpeed * Time.deltaTime);

            velocity.x = Mathf.Lerp(velocity.x, currentMovementSpeed * targetForwardDirection.x, acceleration.x * Time.deltaTime);
            velocity.z = Mathf.Lerp(velocity.z, currentMovementSpeed * targetForwardDirection.z, acceleration.z * Time.deltaTime);
        }
        else
        {
            velocity.x = Mathf.Lerp(velocity.x, 0f, acceleration.x * Time.deltaTime);
            velocity.z = Mathf.Lerp(velocity.z, 0f, acceleration.z * Time.deltaTime);
        }

        Vector3 groundedVelocity = new Vector3(velocity.x, 0f, velocity.z);
        groundedVelocity = Vector3.ClampMagnitude(groundedVelocity, currentMovementSpeed);

        controller.Move(groundedVelocity * Time.deltaTime);
    }

    private void HandleGravity()
    {
        if (IsGrounded)
        {
            inAirTimer = 0f;
            if(velocity.y < groundedYVelocity)
            {
                IsJumping = false;
                currentJumpCount = 0;
                fallVelocityApplied = false;
            }
        }

        if(!IsGrounded)
        {
            if (!IsJumping && !fallVelocityApplied)
            {
                fallVelocityApplied = true;
                velocity.y = fallingStartingYVelocity;
            }
            inAirTimer += Time.deltaTime;
            velocity.y += acceleration.y * Time.deltaTime;
        }

        controller.Move(Time.deltaTime * velocity.y * Vector3.up);
    }

    private void HandleSpeed()
    {
        if (!IsMoving)
        {
            currentMovementSpeed = Mathf.Lerp(currentMovementSpeed, 0f, 10f * Time.deltaTime);
            return;
        }

        if (IsSprinting)
        {
            currentMovementSpeed = Mathf.Lerp(currentMovementSpeed, maxSpeed, 10f * Time.deltaTime);
        }

        if (!IsSprinting)
        {
            currentMovementSpeed = Mathf.Lerp(currentMovementSpeed, walkSpeed, 10f * Time.deltaTime);
        }


    }

    private void HandleAnimations()
    {
        animator.SetFloat("MovementSpeed", currentMovementSpeed/maxSpeed);
        animator.SetFloat("InAirTimer", inAirTimer);
        animator.SetFloat("AttackAnimationSpeedMultiplier", attackAnimationSpeedMultiplier);
        animator.SetBool("IsGrounded", IsGrounded);

        if (IsAttacking && !CanMove)
        {
            Quaternion targetRotation = Quaternion.Euler(0, instantaneousAttackAngle, 0);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 2f * rotationSpeed * Time.deltaTime);
        }

        dashTrailObject.SetActive(IsDashing);
    }

    private void SwingMeleeWeapon(string animationName)
    {
        if (currentSwingingCoroutine != null) StopCurrentSwingCoroutine();
        currentSwingingCoroutine = StartCoroutine(SwingCoroutine(animationName, maxComboDelay));
    }

    private IEnumerator SwingCoroutine(string animationName, float animationFadeSpeed)
    {
        weapon.ClearEnemiesHitList();

        instantaneousAttackAngle = Mathf.Atan2(input.MoveDirection.x, input.MoveDirection.z) * Mathf.Rad2Deg + Camera.main.transform.rotation.eulerAngles.y;

        IsAttacking = true;

        animator.CrossFadeInFixedTime(animationName, 0.05f);

        float animationDuration = GetAnimationDuration(animationName) / attackAnimationSpeedMultiplier;
        yield return new WaitForSeconds(animationDuration);

        animator.CrossFadeInFixedTime("FlatMovement", animationFadeSpeed);

        if (comboIndex == weapon.Combo.PrimaryCombo.Count - 1)
        {
            comboIndex = 0;

            CanMove = true;
            IsAttacking = false;

            yield return new WaitForSeconds(0.3f);

            yield break;
        }

        for (float t = 0; t < maxComboDelay; t += Time.unscaledDeltaTime)
        {
            if (input.Attack)
            {
                if (!CanAttack)
                {
                    StopCurrentSwingCoroutine();
                    yield break;
                }

                comboIndex++;

                StopCoroutine(currentSwingingCoroutine);
                currentSwingingCoroutine = StartCoroutine(SwingCoroutine(weapon.Combo.PrimaryCombo[comboIndex].AnimationClipName, maxComboDelay));
                yield break;
            }

            yield return null;
        }

        IsAttacking = false;

        comboIndex = 0;
    }

    private void StopCurrentSwingCoroutine()
    {
        if (currentSwingingCoroutine != null) StopCoroutine(currentSwingingCoroutine);

        animator.CrossFadeInFixedTime("FlatMovement", 0.1f);

        comboIndex = 0;

        weapon.DisableTriggers();
        CanMove = true;
        IsAttacking = false;
    }

    private void Dash()
    {
        if (IsDashing) return;

        if (currentSwingingCoroutine != null) StopCurrentSwingCoroutine();

        if (dashCoroutine != null) StopDashing();
        dashCoroutine = StartCoroutine(DashCoroutine());
        dashDelayTimer = 0f;
    }

    private IEnumerator DashCoroutine()
    {
        IsDashing = true;

        if (IsGrounded) animator.CrossFadeInFixedTime("Dash", 0.1f);

        float currDashVelocity = initialDashVelocity;
        for (float t = 0; t < dashDuration; t += Time.deltaTime)
        {
            currDashVelocity = (initialDashVelocity - maxSpeed) * (1 - Mathf.Sqrt(1 - Mathf.Pow(t / dashDuration - 1, 2))) + maxSpeed;

            velocity.x = currDashVelocity * targetForwardDirection.x;
            velocity.z = currDashVelocity * targetForwardDirection.z;

            transform.rotation = Quaternion.Slerp(transform.rotation, targetForwardRotation, rotationSpeed * Time.deltaTime);
            controller.Move(new Vector3(velocity.x, 0f, velocity.z) * Time.deltaTime);
            yield return null;
        }

        IsDashing = false;
        IsSprinting = true;

        dashDelayTimer = 0f;

        if (IsGrounded) animator.CrossFadeInFixedTime("FlatMovement", 0.1f);

        for(float t = 0; t < sprintDurationAfterDash; t += Time.deltaTime)
        {
            if (!IsMoving) break;
            yield return null;
        }

        IsSprinting = false;
    }

    private void StopDashing()
    {
        if (dashCoroutine != null) StopCoroutine(dashCoroutine);

        IsDashing = false;
        CanMove = true;
    }

    private void HandleWeaponCollisions()
    {
        if (!IsAttacking)
        {
            DisableWeaponTriggers();
        }
    }

    public void EnableWeaponTriggers()
    {
        weapon.EnableTriggers();
    }

    public void DisableWeaponTriggers()
    {
        weapon.DisableTriggers();
    }

    private float GetAnimationDuration(string animationName)
    {
        AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;

        if (clips == null) return 0f;
        if (clips.Length == 0) return 0f;

        foreach(AnimationClip clip in clips)
        {
            if (clip.name == animationName) return clip.length;
        }

        return 0f;
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
