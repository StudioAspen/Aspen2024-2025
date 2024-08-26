using KBCore.Refs;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

    private void OnValidate()
    {
        this.ValidateRefs();
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

        if (input.Attack)
        {
            SwingMeleeWeapon(weapon.Combo.PrimaryCombo[comboIndex].AnimationClipName);
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
        if (input.Attack)
        {
            if (currentSwingingCoroutine != null) CancelCurrentSwing();
            currentSwingingCoroutine = StartCoroutine(SwingCoroutine(animationName, maxComboDelay));
        }
    }

    private IEnumerator SwingCoroutine(string animationName, float animationFadeSpeed)
    {
        comboTimer = 0f;

        weapon.ClearEnemiesHitList();

        instantaneousAttackAngle = Mathf.Atan2(input.MoveDirection.x, input.MoveDirection.z) * Mathf.Rad2Deg + Camera.main.transform.rotation.eulerAngles.y;

        player.IsAttacking = true;

        animator.CrossFadeInFixedTime(animationName, 0.05f);

        float animationDuration = GetAnimationDuration(animationName) / attackAnimationSpeedMultiplier;
        yield return new WaitForSeconds(animationDuration);

        animator.CrossFadeInFixedTime("FlatMovement", animationFadeSpeed);

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
        if (currentSwingingCoroutine != null) StopCoroutine(currentSwingingCoroutine);

        animator.CrossFadeInFixedTime("FlatMovement", 0.1f);

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

    private float GetAnimationDuration(string animationName)
    {
        AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;

        if (clips == null) return 0f;
        if (clips.Length == 0) return 0f;

        foreach (AnimationClip clip in clips)
        {
            if (clip.name == animationName) return clip.length;
        }

        return 0f;
    }
}
