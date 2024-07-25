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
    [HideInInspector] public bool IsAttacking;
    [HideInInspector] public bool IsSwinging;
    #endregion

    [Header("Combat")]
    [SerializeField] private Transform swordAnchorTransform;
    [SerializeField] private float maxComboDelay = 0.3f;
    [SerializeField] private int maxComboCount = 3;
    private float instantaneousAttackAngle;
    private int comboIndex;
    public float comboDelayTimer;
    private Coroutine currentSwingingCoroutine;
    private Coroutine slowTimeCoroutine;
    private List<Enemy> enemiesHitByCurrentAttack = new List<Enemy>();

    [Header("Camera")]
    public bool CameraLocked = true;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        HandleFlatMovement();
        CheckGrounded();
        HandleJump();
        HandleGravity();
        HandleSpeed();

        HandleAnimations();

        HandleSwing();
        HandleAttacking();

        Cursor.lockState = CameraLocked ? CursorLockMode.Locked : CursorLockMode.None;
    }

    private void OnDrawGizmos()
    {

    }

    private void HandleFlatMovement()
    {
        if (!CanMove)
        {
            currentMovementSpeed = 0f;
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
            transform.Translate(currentMovementSpeed * targetDirection * Time.deltaTime, Space.World);
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

    private void HandleSpeed()
    {
        IsSprinting = Input.GetKey(KeyCode.LeftShift) && IsMoving;

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
            currentSwingingCoroutine = StartCoroutine(SwingCoroutine("Slash1", 0.25f));
        }
    }

    private void HandleAttacking()
    {
        if (IsAttacking && !CanMove)
        {
            Quaternion targetRotation = Quaternion.Euler(0, instantaneousAttackAngle, 0);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 2f * rotationSpeed * Time.deltaTime);
        }

        if (IsSwinging)
        {
            Debug.DrawLine(swordAnchorTransform.position, swordAnchorTransform.position + 1.5f * swordAnchorTransform.up);
            RaycastHit[] hits = Physics.SphereCastAll(swordAnchorTransform.position, 0.2f, swordAnchorTransform.up, 1.5f);
            if (hits == null) return;
            if (hits.Length == 0) return;

            foreach(RaycastHit hit in hits)
            {
                if(hit.collider.gameObject.TryGetComponent(out Enemy enemy))
                {
                    if (enemiesHitByCurrentAttack.Contains(enemy)) return;
                    enemiesHitByCurrentAttack.Add(enemy);

                    if (slowTimeCoroutine != null) StopCoroutine(slowTimeCoroutine);
                    slowTimeCoroutine = StartCoroutine(SlowTimePerHitCoroutine(0.025f, 0.15f));
                    CameraShakeManager.Instance.ShakeCamera(5f, 0.25f);

                    GameObject temp = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    temp.GetComponent<Collider>().enabled = false;
                    temp.transform.localScale = 0.25f * Vector3.one;
                    if (hit.distance == 0)
                    {
                        temp.transform.position = hit.collider.ClosestPoint(swordAnchorTransform.position);
                    }
                    else
                    {
                        temp.transform.position = hit.point;
                    }
                    temp.GetComponent<Renderer>().material.color = Color.red;
                    Destroy(temp, 2f);
                    
                    enemy.TakeDamage(1);
                }
            }
        }
    }

    private IEnumerator SlowTimePerHitCoroutine(float timeScale, float duration)
    {
        Time.timeScale = timeScale;

        yield return new WaitForSecondsRealtime(duration);

        Time.timeScale = 1f;
    }

    private IEnumerator SwingCoroutine(string animationName, float animationFadeSpeed)
    {
        //Debug.Log($"Combo {comboIndex}");

        enemiesHitByCurrentAttack.Clear();

        comboDelayTimer = 0f;

        instantaneousAttackAngle = Camera.main.transform.rotation.eulerAngles.y;

        CanMove = false;
        IsAttacking = true;
        IsSwinging = true;

        animator.CrossFadeInFixedTime(animationName, animationFadeSpeed);

        float animationDuration = GetAnimationDuration(animationName);
        yield return new WaitForSeconds(animationDuration);

        IsSwinging = false;

        animator.CrossFadeInFixedTime("FlatMovement", animationFadeSpeed);

        if (comboIndex == maxComboCount - 1)
        {
            comboIndex = 0;

            CanMove = true;
            IsAttacking = false;

            //Debug.Log("Broke Combo, Combo delay waiting");
            yield return new WaitForSeconds(0.5f);

            yield break;
        }

        CanMove = true;

        for(float t = 0; t < maxComboDelay; t += Time.deltaTime)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                comboIndex++;

                if(comboIndex == 1)
                {
                    StopCoroutine(currentSwingingCoroutine);
                    currentSwingingCoroutine = StartCoroutine(SwingCoroutine("Slash2", 0.25f));
                    yield break;
                }
            }

            yield return null;
        }

        IsAttacking = false;

        Debug.Log("Broke combo");
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
}
