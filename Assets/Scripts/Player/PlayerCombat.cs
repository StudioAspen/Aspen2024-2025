using KBCore.Refs;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Unity.VisualScripting;
using UnityEditor.Animations;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("References")]
    [SerializeField, Self] private InputReader input;
    [SerializeField, Self] private PlayerController player;
    [SerializeField, Self] private Animator animator;

    [Header("Settings")]
    [SerializeField] private WeaponHandler weapon;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private float maxComboDelay = 0.5f;
    [SerializeField] private float attackAnimationSpeedMultiplier = 3f;

    [SerializeField] private float comboTimer;
    private float instantaneousAttackAngle;
    [SerializeField] private int comboIndex;
    private Coroutine currentSwingingCoroutine;

    private string[] basicSwingAnimationClipNames = { "BasicSwing1", "BasicSwing2", "BasicSwing3" };

    private void OnValidate()
    {
        this.ValidateRefs();
    }

    private void Awake()
    {

    }

    private void Update()
    {
        HandleAnimations();
        RotateTowardsAttackAngle();

        HandleComboTimer();

        HandleSwingInput();

        HandleWeaponCollisions();
    }

    private void HandleComboTimer()
    {
        if (!player.IsAttacking && comboTimer < maxComboDelay * 2f) comboTimer += Time.deltaTime;
        
        if (comboTimer > maxComboDelay)
        {
            comboIndex = 0;
        }
    }

    private void HandleAnimations()
    {
        animator.SetFloat("AttackAnimationSpeedMultiplier", attackAnimationSpeedMultiplier);
    }

    private void HandleSwingInput()
    {
        if (!player.CanAttack) return;
        if (player.IsAttacking) return;

        if (input.BasicAttack)
        {
            SwingMeleeWeapon(basicSwingAnimationClipNames[comboIndex]);
        }
    }

    private void RotateTowardsAttackAngle()
    {
        if (player.IsAttacking)
        {
            Quaternion targetRotation = Quaternion.Euler(0, instantaneousAttackAngle, 0);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 2f * rotationSpeed * Time.deltaTime);
        }
    }

    private void SwingMeleeWeapon(string animationName)
    {
        if (currentSwingingCoroutine != null) CancelCurrentSwing();
        currentSwingingCoroutine = StartCoroutine(SwingCoroutine(basicSwingAnimationClipNames[comboIndex], maxComboDelay));
    }

    private IEnumerator SwingCoroutine(string animationName, float animationFadeSpeed)
    {
        comboTimer = 0f;

        weapon.ClearEnemiesHitList();

        instantaneousAttackAngle = Mathf.Atan2(input.MoveDirection.x, input.MoveDirection.z) * Mathf.Rad2Deg + Camera.main.transform.rotation.eulerAngles.y;

        player.IsAttacking = true;

        animator.CrossFadeInFixedTime(animationName, 0.05f, animator.GetLayerIndex("UpperBody"));

        // Wait until the attack starts playing
        while (!animator.GetCurrentAnimatorStateInfo(animator.GetLayerIndex("UpperBody")).IsName(animationName))
        {
            yield return null;
        }

        float animationDuration = animator.GetCurrentAnimatorStateInfo(animator.GetLayerIndex("UpperBody")).length;
        yield return new WaitForSeconds(animationDuration);

        animator.CrossFadeInFixedTime("FlatMovement", animationFadeSpeed, animator.GetLayerIndex("UpperBody"));

        if (comboIndex == weapon.Combo.PrimaryCombo.Count - 1)
        {
            comboIndex = 0;

            player.IsAttacking = false;

            yield break;
        }

        comboIndex++;

        player.IsAttacking = false;
    }

    private void CancelCurrentSwing()
    {
        StopCoroutine(currentSwingingCoroutine);

        animator.CrossFadeInFixedTime("FlatMovement", 0.1f, animator.GetLayerIndex("UpperBody"));

        weapon.DisableTriggers();
        player.IsAttacking = false;
    }

    private void HandleWeaponCollisions()
    {
        if (!player.IsAttacking)
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
}
