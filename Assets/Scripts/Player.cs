using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Animator animator;

    [Header("Player Speed")]
    [SerializeField] private float currentMovementSpeed;
    [SerializeField] private float walkSpeed = 3f;
    [SerializeField] private float sprintSpeed = 5f;
    [SerializeField] private float rotationSpeed = 5f;

    [Header("Player Gravity")]
    [SerializeField] private float jumpHeight = 3f;
    [SerializeField] private float gravity = -9.81f;
    private float velocityY;

    #region Flags
    [HideInInspector] public bool IsGrounded;
    [HideInInspector] public bool CanMove = true;
    [HideInInspector] public bool IsMoving;
    [HideInInspector] public bool IsSprinting;
    [HideInInspector] public bool IsDashing;
    [HideInInspector] public bool IsAttacking;
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

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        HandleFlatMovement();
        HandleSpeed();
        HandleSprint();
        HandleDash();

        CheckGrounded();
        HandleJump();
        HandleGravity();

        HandleAnimations();

        HandleSwing();
        HandleAttacking();

        Cursor.lockState = CameraLocked ? CursorLockMode.Locked : CursorLockMode.None;
    }

    private void HandleFlatMovement()
    {
        if (!CanMove)
        {
            currentMovementSpeed = 0f;
            return;
        }

        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        Vector3 movementDirection = new Vector3(x, 0, z);

        IsMoving = movementDirection.magnitude > 0;

        if (IsMoving)
        {
            float angle = Mathf.Atan2(x, z) * Mathf.Rad2Deg + Camera.main.transform.rotation.eulerAngles.y;
            Quaternion targetRotation = Quaternion.Euler(0, angle, 0);
            Vector3 targetDirection = targetRotation * Vector3.forward;

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            transform.Translate(currentMovementSpeed * movementDirection.magnitude * targetDirection * Time.deltaTime, Space.World);
        }

    }

    private void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded)
        {
            velocityY = Mathf.Sqrt(-2f * gravity * jumpHeight);
            IsGrounded = false;
        }
    }

    private void HandleGravity()
    {
        float displacement = velocityY * Time.deltaTime + 0.5f * gravity * (Time.deltaTime * Time.deltaTime);

        velocityY += gravity * Time.deltaTime;

        if (IsGrounded)
        {
            velocityY = 0f;
            return;
        }

        transform.Translate(displacement * Vector3.up);
    }

    private void CheckGrounded()
    {
        float distance = 0.1f;
        Vector3 upOffset = distance * Vector3.up;

        bool middle = Physics.Raycast(transform.position + upOffset, Vector3.down, distance);

        CapsuleCollider c = GetComponent<CapsuleCollider>();

        bool left = Physics.Raycast(transform.position + c.radius * Vector3.up + c.radius * Vector3.left + upOffset, Vector3.down, distance);
        bool right = Physics.Raycast(transform.position + c.radius * Vector3.up + c.radius * Vector3.right + upOffset, Vector3.down, distance);
        bool forward = Physics.Raycast(transform.position + c.radius * Vector3.up + c.radius * Vector3.forward + upOffset, Vector3.down, distance);
        bool backward = Physics.Raycast(transform.position + c.radius * Vector3.up + c.radius * Vector3.back + upOffset, Vector3.down, distance);
        
        IsGrounded = middle || left || right || forward || backward;
    }

    private void HandleAnimations()
    {
        animator.SetFloat("MovementSpeed", currentMovementSpeed/sprintSpeed);
    }

    private void HandleSprint()
    {
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

    private void HandleSwing()
    {
        if (IsAttacking) return;

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            currentSwingingCoroutine = StartCoroutine(SwingCoroutine(weapon.Combo.WeaponSwings[comboIndex].AnimationClipName, maxComboDelay));
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

    private IEnumerator SlowTimePerHitCoroutine(float timeScale, float duration)
    {
        float speedUpTime = duration / 4;

        Time.timeScale = timeScale;

        yield return new WaitForSecondsRealtime(duration - speedUpTime);

        for (float t = 0; t < speedUpTime; t += Time.unscaledDeltaTime)
        {
            Time.timeScale = Mathf.Lerp(timeScale, 1f, t / speedUpTime);
            yield return null;
        }

        Time.timeScale = 1f;
    }

    private IEnumerator SwingCoroutine(string animationName, float animationFadeSpeed)
    {
        weapon.ClearEnemiesHitList();

        instantaneousAttackAngle = Camera.main.transform.rotation.eulerAngles.y;

        CanMove = false;
        IsAttacking = true;

        animator.CrossFadeInFixedTime(animationName, 0.05f);

        float timeToStartDamage = weapon.Combo.WeaponSwings[comboIndex].TimeToStartDamage;
        yield return new WaitForSeconds(timeToStartDamage);
        weapon.EnableTriggers();

        float timeToStopDamage = weapon.Combo.WeaponSwings[comboIndex].TimeToStopDamage;
        yield return new WaitForSeconds(timeToStopDamage - timeToStartDamage);
        weapon.DisableTriggers();

        float animationDuration = GetAnimationDuration(animationName);
        yield return new WaitForSeconds(animationDuration - timeToStopDamage);

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
            IsSprinting = false;

            if (shiftKeyPressTimer < shiftKeyPressDurationThresholdForSprint)
            {
                if(dashCoroutine != null) StopCoroutine(dashCoroutine);
                dashCoroutine = StartCoroutine(DashCoroutine());
            }
            shiftKeyPressTimer = 0f;
        }
    }

    private IEnumerator DashCoroutine()
    {
        IsDashing = true;

        float currDashVelocity = initialDashVelocity;
        for(float t = 0; t < dashDuration; t += Time.deltaTime)
        {
            currDashVelocity = initialDashVelocity * (1 - Mathf.Sqrt(1-Mathf.Pow(t/dashDuration - 1, 2)));

            currentMovementSpeed = currDashVelocity + sprintSpeed;


            float x = Input.GetAxisRaw("Horizontal");
            float z = Input.GetAxisRaw("Vertical");

            Vector3 movementDirection = new Vector3(x, 0, z);

            float angle = Mathf.Atan2(x, z) * Mathf.Rad2Deg + Camera.main.transform.rotation.eulerAngles.y;
            Quaternion targetRotation = Quaternion.Euler(0, angle, 0);
            Vector3 targetDirection = targetRotation * Vector3.forward;

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            transform.Translate(currDashVelocity * targetDirection * Time.deltaTime, Space.World);
            yield return null;
        }

        currentMovementSpeed = sprintSpeed;
        IsSprinting = true;
        IsDashing = false;

        dashDelayTimer = 0f;

        for(float t = 0; t < sprintDurationAfterDash; t += Time.deltaTime)
        {
            if (!IsMoving)
            {
                IsSprinting = false;
                yield break;
            }
            yield return null;
        }

        IsSprinting = false;
    }
}
