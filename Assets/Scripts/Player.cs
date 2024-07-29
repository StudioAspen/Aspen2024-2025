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
    [SerializeField] private float walkSpeed = 3f;
    [SerializeField] private float sprintSpeed = 5f;
    [SerializeField] private float rotationSpeed = 5f;
    private Vector3 movementDirection;

    [Header("Player Gravity")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float jumpHeight = 3f;
    [SerializeField] private float gravityForce = -9.81f;
    private float yVelocity;
    private float inAirTimer = 0;

    #region Flags
    [HideInInspector] public bool IsGrounded;
    [HideInInspector] public bool CanMove = true;
    [HideInInspector] public bool IsMoving;
    [HideInInspector] public bool IsSprinting;
    [HideInInspector] public bool IsDashing;
    [HideInInspector] public bool IsAttacking;
    [HideInInspector] public bool IsJumping;
    #endregion

    [Header("Combat")]
    [SerializeField] private WeaponHandler weapon;
    private float instantaneousAttackAngle;
    [SerializeField] private float maxComboDelay = 0.5f;
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
        HandleFlatMovement();
        HandleSpeed();
        HandleSprint();
        HandleDash();

        CheckGrounded();
        HandleJumpInput();
        HandleGravity();

        HandleAnimations();

        HandleSwingInput();
        HandleAttacking();

        Cursor.lockState = CameraLocked ? CursorLockMode.Locked : CursorLockMode.None;
    }

    private void HandleFlatMovement()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        movementDirection = new Vector3(x, 0, z);

        if (!CanMove)
        {
            currentMovementSpeed = 0f;
            IsMoving = false;
            return;
        }

        IsMoving = movementDirection.magnitude > 0;

        if (IsMoving)
        {
            float angle = Mathf.Atan2(x, z) * Mathf.Rad2Deg + Camera.main.transform.rotation.eulerAngles.y;
            Quaternion targetRotation = Quaternion.Euler(0, angle, 0);
            Vector3 targetDirection = targetRotation * Vector3.forward;

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            characterController.Move(currentMovementSpeed * targetDirection * Time.deltaTime);
        }
    }

    private void HandleJumpInput()
    {
        if (!IsGrounded) return;
        if (IsJumping) return;
        if (IsAttacking) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            yVelocity = 0f;
            yVelocity += Mathf.Sqrt(jumpHeight * -2f * gravityForce);

            IsJumping = true;
            animator.CrossFadeInFixedTime("JumpingUp", 0.1f);
        }
    }

    private void HandleGravity()
    {
        if (IsGrounded)
        {
            inAirTimer = 0f;
            IsJumping = false;
        }

        if(IsGrounded && yVelocity < 0f)
        {
            yVelocity = 0f;
        }

        if(!IsGrounded && !IsJumping)
        {
            inAirTimer += Time.deltaTime;
        }

        yVelocity += gravityForce * Time.deltaTime;

        characterController.Move(yVelocity * Time.deltaTime * Vector3.up);
    }

    public void ApplyJumpingVelocity()
    {
        //yVelocity = Mathf.Sqrt(jumpHeight * -2f * gravityForce);
    }

    private void CheckGrounded()
    {
        IsGrounded = Physics.CheckSphere(transform.position + characterController.radius/2 * Vector3.up, characterController.radius, groundLayer);
    }

    private void HandleAnimations()
    {
        animator.SetFloat("MovementSpeed", currentMovementSpeed/sprintSpeed);
        animator.SetFloat("InAirTimer", inAirTimer);
        animator.SetBool("IsGrounded", IsGrounded);
    }

    private void HandleSprint()
    {
        if (!IsGrounded) return;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            shiftKeyPressTimer += Time.deltaTime;
            if (!IsMoving)
            {
                IsSprinting = false;
                return;
            }
            IsSprinting = true;
        }
    }

    private void HandleSpeed()
    {
        if (!IsMoving)
        {
            IsSprinting = false;
        }

        if (IsSprinting)
        {
            currentMovementSpeed = Mathf.Lerp(currentMovementSpeed, sprintSpeed, 10f * Time.deltaTime);
        }

        if(IsMoving && !IsSprinting)
        {
            currentMovementSpeed = Mathf.Lerp(currentMovementSpeed, walkSpeed, 10f * Time.deltaTime);
        }

        if (!IsMoving)
        {
            currentMovementSpeed = Mathf.Lerp(currentMovementSpeed, 0f, 10f * Time.deltaTime);
        }
    }

    private void HandleSwingInput()
    {
        if (!IsGrounded) return;
        if (IsAttacking) return;

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            SwingMeleeWeapon(weapon.Combo.WeaponSwings[comboIndex].AnimationClipName);
        }
    }

    private void HandleAttacking()
    {
        if (IsAttacking && !CanMove)
        {
            Quaternion targetRotation = Quaternion.Euler(0, instantaneousAttackAngle, 0);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 2f * rotationSpeed * Time.deltaTime);
        }
    }

    private void SwingMeleeWeapon(string animationName)
    {
        if (dashCoroutine != null) StopDashing();

        if (currentSwingingCoroutine != null) StopCurrentSwingCoroutine();
        currentSwingingCoroutine = StartCoroutine(SwingCoroutine(animationName, maxComboDelay));
    }

    private IEnumerator SwingCoroutine(string animationName, float animationFadeSpeed)
    {
        weapon.ClearEnemiesHitList();

        instantaneousAttackAngle = Mathf.Atan2(movementDirection.x, movementDirection.z) * Mathf.Rad2Deg + Camera.main.transform.rotation.eulerAngles.y;

        CanMove = false;
        IsAttacking = true;

        animator.CrossFadeInFixedTime(animationName, 0.05f);

        float animationDuration = GetAnimationDuration(animationName);
        yield return new WaitForSeconds(animationDuration);

        animator.CrossFadeInFixedTime("FlatMovement", animationFadeSpeed);

        if (comboIndex == weapon.Combo.WeaponSwings.Count - 1)
        {
            comboIndex = 0;

            CanMove = true;
            IsAttacking = false;

            yield return new WaitForSeconds(0.3f);

            yield break;
        }

        CanMove = true;

        for(float t = 0; t < maxComboDelay; t += Time.unscaledDeltaTime)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                comboIndex++;

                StopCoroutine(currentSwingingCoroutine);
                currentSwingingCoroutine = StartCoroutine(SwingCoroutine(weapon.Combo.WeaponSwings[comboIndex].AnimationClipName, maxComboDelay));
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

    private void HandleDash()
    {
        dashDelayTimer += Time.deltaTime;

        if (dashDelayTimer < dashDelayDuration) return;
        if (IsDashing) return;

        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            if (!IsGrounded) return;

            IsSprinting = false;

            if (shiftKeyPressTimer < shiftKeyPressDurationThresholdForSprint)
            {
                Dash();
            }
            shiftKeyPressTimer = 0f;
        }
    }

    private void Dash()
    {
        if (currentSwingingCoroutine != null) StopCurrentSwingCoroutine();

        if (dashCoroutine != null) StopDashing();
        dashCoroutine = StartCoroutine(DashCoroutine());
    }

    private void StopDashing()
    {
        if (dashCoroutine != null) StopCoroutine(dashCoroutine);

        IsDashing = false;
        IsSprinting = false;
        CanMove = true;
    }

    private IEnumerator DashCoroutine()
    {
        IsDashing = true;
        CanMove = false;
        animator.CrossFadeInFixedTime("Dash", 0.1f);

        float currDashVelocity = initialDashVelocity;
        for(float t = 0; t < dashDuration; t += Time.deltaTime)
        {
            currDashVelocity = initialDashVelocity * (1 - Mathf.Sqrt(1-Mathf.Pow(t/dashDuration - 1, 2)));

            float angle = Mathf.Atan2(movementDirection.x, movementDirection.z) * Mathf.Rad2Deg + Camera.main.transform.rotation.eulerAngles.y;
            Quaternion targetRotation = Quaternion.Euler(0, angle, 0);
            Vector3 targetDirection = targetRotation * Vector3.forward;

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            characterController.Move(currDashVelocity * targetDirection * Time.deltaTime);
            yield return null;
        }

        IsDashing = false;
        CanMove = true;
        IsSprinting = true;
        currentMovementSpeed = sprintSpeed;

        animator.CrossFadeInFixedTime("FlatMovement", 0.1f);

        dashDelayTimer = 0f;

        for(float t = 0; t < sprintDurationAfterDash; t += Time.deltaTime)
        {
            if (movementDirection.magnitude == 0)
            {
                IsSprinting = false;
                yield break;
            }
            yield return null;
        }

        IsSprinting = false;
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
