using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Animator animator;
    private CharacterController characterController;

    [Header("Player Speed")]
    [SerializeField] private float currentMovementSpeed;
    [SerializeField] private float maxSpeed = 5f;
    [SerializeField] private float rotationSpeed = 5f;
    private Vector3 movementDirection;

    [Header("Player Gravity")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float jumpHeight = 3f;
    [SerializeField] private int maxJumpCount = 2;
    [SerializeField] private Vector3 acceleration = new Vector3(0f, -9.81f, 0f);
    [SerializeField] private Vector3 velocity;
    private float inAirTimer = 0;
    private int currentJumpCount;

    #region Flags
    [HideInInspector] public bool IsGrounded;
    [HideInInspector] public bool CanMove = true;
    [HideInInspector] public bool IsMoving;
    [HideInInspector] public bool IsDashing;
    [HideInInspector] public bool IsAttacking;
    [HideInInspector] public bool CanAttack = true;
    [HideInInspector] public bool IsJumping;
    #endregion

    [Header("Combat")]
    [SerializeField] private WeaponHandler weapon;
    private float instantaneousAttackAngle;
    [SerializeField] private float maxComboDelay = 0.5f;
    [SerializeField] private float attackAnimationSpeedMultiplier = 3f;
    private int comboIndex;
    private Coroutine currentSwingingCoroutine;

    [Header("Dash")]
    [SerializeField] private float dashDuration = 1f;
    [SerializeField] private float initialDashVelocity = 5f;
    [SerializeField] private float dashDelayDuration = 1f;
    [SerializeField] private float sprintDurationAfterDash = 2f;
    [SerializeField] private float shiftKeyPressDurationThresholdForSprint = 0.25f;
    private float shiftKeyPressTimer;
    private float dashDelayTimer = Mathf.Infinity;
    private Coroutine dashCoroutine;

    [Header("Camera")]
    public bool CameraLocked = true;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();

        QualitySettings.vSyncCount = 0;  // VSync must be disabled
        Application.targetFrameRate = 0;
    }

    void Start()
    {
        IgnoreMyOwnColliders();
    }

    void Update()
    {
        HandleGroundedMovement();
        HandleDashInput();

        CheckGrounded();
        HandleJumpInput();
        HandleGravity();

        HandleAnimations();

        HandleSwingInput();

        Cursor.lockState = CameraLocked ? CursorLockMode.Locked : CursorLockMode.None;
    }

    private void HandleGroundedMovement()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        movementDirection = new Vector3(x, 0, z);

        if (!CanMove)
        {
            return;
        }

        Vector3 groundedVelocity = new Vector3(velocity.x, 0f, velocity.z);

        IsMoving = movementDirection.magnitude > 0;

        Vector3 targetDirection = Vector3.zero;

        if (IsMoving)
        {
            float angle = Mathf.Atan2(movementDirection.x, movementDirection.z) * Mathf.Rad2Deg + Camera.main.transform.rotation.eulerAngles.y;
            Quaternion targetRotation = Quaternion.Euler(0, angle, 0);
            targetDirection = targetRotation * Vector3.forward;

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            velocity.x = Mathf.Lerp(velocity.x, maxSpeed * targetDirection.x, acceleration.x * Time.deltaTime);
            velocity.z = Mathf.Lerp(velocity.z, maxSpeed * targetDirection.z, acceleration.z * Time.deltaTime);
        }
        else
        {
            velocity.x = Mathf.Lerp(velocity.x, 0f, acceleration.x * Time.deltaTime);
            velocity.z = Mathf.Lerp(velocity.z, 0f, acceleration.z * Time.deltaTime);
        }

        groundedVelocity = Vector3.ClampMagnitude(groundedVelocity, maxSpeed);
        currentMovementSpeed = groundedVelocity.magnitude;
        characterController.Move(groundedVelocity * Time.deltaTime);
    }

    private void HandleJumpInput()
    {
        if (!IsGrounded && currentJumpCount == maxJumpCount) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            velocity.y = 0f;
            velocity.y += Mathf.Sqrt(jumpHeight * -2f * acceleration.y);

            IsJumping = true;
            IsGrounded = false;
            animator.CrossFadeInFixedTime("JumpingUp", 0.1f);

            currentJumpCount++;
        }
    }

    private void HandleGravity()
    {
        if (IsGrounded)
        {
            inAirTimer = 0f;
            IsJumping = false;
            currentJumpCount = 0;
        }

        if(IsGrounded && velocity.y < 0f)
        {
            velocity.y = 0f;
        }

        if(!IsGrounded)
        {
            inAirTimer += Time.deltaTime;
        }

        velocity.y += acceleration.y * Time.deltaTime;

        characterController.Move(Time.deltaTime * velocity.y * Vector3.up);
    }

    private void CheckGrounded()
    {
        IsGrounded = Physics.CheckSphere(transform.position + characterController.radius/2 * Vector3.up, characterController.radius, groundLayer);
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
    }

    private void HandleDashInput()
    {
        dashDelayTimer += Time.deltaTime;

        if (dashDelayTimer < dashDelayDuration) return;

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            Dash();
        }
    }

    private void HandleSwingInput()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (!CanAttack) return;
            if (IsAttacking) return;

            SwingMeleeWeapon(weapon.Combo.PrimaryCombo[comboIndex].AnimationClipName);
        }
    }

    private void SwingMeleeWeapon(string animationName)
    {
        if (dashCoroutine != null) StopDashing();

        if (currentSwingingCoroutine != null) StopCurrentSwingCoroutine();
        currentSwingingCoroutine = StartCoroutine(SwingCoroutine(animationName, maxComboDelay));
    }

    private void PerformJumpAttack(string animationName)
    {
        if (currentSwingingCoroutine != null) StopCurrentSwingCoroutine();
        currentSwingingCoroutine = StartCoroutine(JumpAttackCoroutine(animationName, 0.2f));
    }

    private IEnumerator SwingCoroutine(string animationName, float animationFadeSpeed)
    {
        weapon.ClearEnemiesHitList();

        instantaneousAttackAngle = Mathf.Atan2(movementDirection.x, movementDirection.z) * Mathf.Rad2Deg + Camera.main.transform.rotation.eulerAngles.y;

        IsAttacking = true;

        animator.CrossFadeInFixedTime(animationName, 0.05f);

        float animationDuration = GetAnimationDuration(animationName)/attackAnimationSpeedMultiplier;
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

        for(float t = 0; t < maxComboDelay; t += Time.unscaledDeltaTime)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
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

    private IEnumerator JumpAttackCoroutine(string animationName, float animationFadeSpeed)
    {
        weapon.ClearEnemiesHitList();

        instantaneousAttackAngle = Mathf.Atan2(movementDirection.x, movementDirection.z) * Mathf.Rad2Deg + Camera.main.transform.rotation.eulerAngles.y;

        CanMove = false;
        IsAttacking = true;

        animator.CrossFadeInFixedTime(animationName, animationFadeSpeed);

        float animationDuration = GetAnimationDuration(animationName);
        yield return new WaitForSeconds(animationDuration);

        CanMove = true;
        IsAttacking = false;
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

    private void Dash()
    {
        if (IsDashing) return;

        if (currentSwingingCoroutine != null) StopCurrentSwingCoroutine();

        if (dashCoroutine != null) StopDashing();
        dashCoroutine = StartCoroutine(DashCoroutine());
        dashDelayTimer = 0f;
    }

    private void StopDashing()
    {
        if (dashCoroutine != null) StopCoroutine(dashCoroutine);

        IsDashing = false;
        CanMove = true;
    }

    private IEnumerator DashCoroutine()
    {
        IsDashing = true;
        CanMove = false;

        if(IsGrounded) animator.CrossFadeInFixedTime("Dash", 0.1f);

        float currDashVelocity = initialDashVelocity;
        for(float t = 0; t < dashDuration; t += Time.deltaTime)
        {
            currDashVelocity = (initialDashVelocity - maxSpeed) * (1 - Mathf.Sqrt(1-Mathf.Pow(t/dashDuration - 1, 2))) + maxSpeed;

            float angle = Mathf.Atan2(movementDirection.x, movementDirection.z) * Mathf.Rad2Deg + Camera.main.transform.rotation.eulerAngles.y;
            Quaternion targetRotation = Quaternion.Euler(0, angle, 0);
            Vector3 targetDirection = targetRotation * Vector3.forward;

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            velocity.x = currDashVelocity * targetDirection.x;
            velocity.z = currDashVelocity * targetDirection.z;

            characterController.Move(new Vector3(velocity.x, 0f, velocity.z) * Time.deltaTime);
            yield return null;
        }

        IsDashing = false;
        CanMove = true;

        if(IsGrounded) animator.CrossFadeInFixedTime("FlatMovement", 0.1f);

        dashDelayTimer = 0f;
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
