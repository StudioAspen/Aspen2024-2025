using KBCore.Refs;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
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
    private float instantaneousAttackAngle;
    private Coroutine currentSwingingCoroutine;

    [Header("Combo")]
    [SerializeField] private float comboListenDuration = 1f;
    [SerializeField] private int maxComboListenCount = 10;
    private float comboListenTimer;
    private List<PlayerActions> currentComboList = new List<PlayerActions>();
    private List<Combo> potentialCombos = new List<Combo>();
    private List<Combo> predictedCombos = new List<Combo>();

    [Header("Input")]
    [SerializeField] private float attackReleaseThreshold = 0.25f;
    private float attack1HoldTimer;
    private float attack2HoldTimer;

    private void OnValidate()
    {
        this.ValidateRefs();
    }

    private void OnEnable()
    {
        input.Attack1Hold.AddListener(HandleAttack1HoldInput);
        input.Attack1Release.AddListener(HandleAttack1ReleaseInput);
        input.Attack2Hold.AddListener(HandleAttack2HoldInput);
        input.Attack2Release.AddListener(HandleAttack2ReleaseInput);

        input.OnPlayerActionInput.AddListener(HandleOnPlayerActionInput);
    }

    private void OnDisable()
    {
        input.Attack1Hold.RemoveListener(HandleAttack1HoldInput);
        input.Attack1Release.RemoveListener(HandleAttack1ReleaseInput);
        input.Attack2Hold.RemoveListener(HandleAttack2HoldInput);
        input.Attack2Release.RemoveListener(HandleAttack2ReleaseInput);

        input.OnPlayerActionInput.RemoveListener(HandleOnPlayerActionInput);
    }

    private void Update()
    {
        RotateTowardsAttackAngle();

        HandleComboList();

        HandleWeaponCollisions();
    }

    private void HandleOnPlayerActionInput(PlayerActions incomingAction)
    {
        comboListenTimer = 0;

        currentComboList.Add(incomingAction);

        GenerateComboLists();

        AttemptToExecuteCombo(incomingAction);
    }

    private void AttemptToExecuteCombo(PlayerActions incomingAction)
    {
        Combo comboToExecute = null;

        if (predictedCombos.Count == 0) // if new action doesn't create any valid combos
        {
            currentComboList.Clear();
            currentComboList.Add(incomingAction);

            comboToExecute = Combo.GetSingleActionCombo(weapon.Combos, incomingAction);
            if (comboToExecute != null)
            {
                animator.SetFloat("ComboAnimationSpeed", comboToExecute.AnimationSpeed);
                ReplaceComboAnimationClip(animator, comboToExecute.AnimationClip);
                SwingMeleeWeapon("Combo");
            }
        }
        else
        {
            comboToExecute = Combo.GetLongestCombo(potentialCombos);

            if (comboToExecute != null)
            {
                animator.SetFloat("ComboAnimationSpeed", comboToExecute.AnimationSpeed);
                ReplaceComboAnimationClip(animator, comboToExecute.AnimationClip);
                SwingMeleeWeapon("Combo");
            }
        }

        PrintComboLists();
    }

    private void GenerateComboLists()
    {
        potentialCombos = new List<Combo>();
        predictedCombos = new List<Combo>();
        foreach (Combo weaponCombo in weapon.Combos)
        {
            if (Combo.IsIn(weaponCombo.Actions, currentComboList)) potentialCombos.Add(weaponCombo);
            if (Combo.IsPotentiallyIn(weaponCombo.Actions, currentComboList)) predictedCombos.Add(weaponCombo);
        }
    }

    private void PrintComboLists()
    {
        string result = "Current Combo: { ";

        for(int i = 0; i < currentComboList.Count; i++)
        {
            result += currentComboList[i].ToString();
            if (i != currentComboList.Count - 1) result += ",";
            result += " ";
        }

        result += "}\nPotential Combos: { ";

        for (int i = 0; i < potentialCombos.Count; i++)
        {
            result += potentialCombos[i].Name;
            if (i != potentialCombos.Count - 1) result += ",";
            result += " ";
        }

        result += "}\nPredicted Combos: { ";

        for (int i = 0; i < predictedCombos.Count; i++)
        {
            result += predictedCombos[i].Name;
            if (i != predictedCombos.Count - 1) result += ",";
            result += " ";
        }

        result += "}";

        Debug.Log(result);
    }

    private void HandleAttack1HoldInput()
    {
        if (!player.CanAttack) return;
        if (player.IsAttacking) return;

        attack1HoldTimer += Time.unscaledDeltaTime;

        if (attack1HoldTimer > attackReleaseThreshold)
        {
            player.IsChargingAttack = true;

            // play the animation matching the predicted potential combo
        }
    }

    private void HandleAttack1ReleaseInput()
    {
        if (!player.CanAttack) return;
        if (player.IsAttacking) return;

        if (attack1HoldTimer < attackReleaseThreshold) // regular swing 1
        {
            input.OnPlayerActionInput?.Invoke(PlayerActions.Attack1);
        }
        else // charged swing 1
        {
            input.OnPlayerActionInput?.Invoke(PlayerActions.ChargeAttack1);
        }

        player.IsChargingAttack = false;
        attack1HoldTimer = 0;
    }

    private void HandleAttack2HoldInput()
    {
        if (!player.CanAttack) return;
        if (player.IsAttacking) return;

        attack2HoldTimer += Time.unscaledDeltaTime;

        if(attack2HoldTimer > attackReleaseThreshold)
        {
            // charge swing 2
        }
    }

    private void HandleAttack2ReleaseInput()
    {
        if (!player.CanAttack) return;
        if (player.IsAttacking) return;

        if (attack2HoldTimer < attackReleaseThreshold)
        {
            // swing 2
        }

        attack2HoldTimer = 0;
    }

    private void HandleComboList()
    {
        if (!player.IsChargingAttack && !player.IsAttacking && comboListenTimer < comboListenDuration * 2f) comboListenTimer += Time.unscaledDeltaTime;

        if (comboListenTimer > comboListenDuration)
        {
            currentComboList.Clear();
            potentialCombos.Clear();
            predictedCombos.Clear();
        }

        if(currentComboList.Count > maxComboListenCount) currentComboList.RemoveAt(0);
    }

    private void ReplaceComboAnimationClip(Animator anim, AnimationClip newClip)
    {
        AnimatorOverrideController aoc = new AnimatorOverrideController(anim.runtimeAnimatorController);

        var anims = new List<KeyValuePair<AnimationClip, AnimationClip>>();

        foreach(AnimationClip currentClip in aoc.animationClips)
        {
            if(currentClip.name == "ComboPlaceholder")
            {
                anims.Add(new KeyValuePair<AnimationClip, AnimationClip>(currentClip, newClip));
            }
        }

        aoc.ApplyOverrides(anims);

        animator.runtimeAnimatorController = aoc;
    }

    private void RotateTowardsAttackAngle()
    {
        if (player.IsMoving) return;

        if (player.IsAttacking)
        {
            Quaternion targetRotation = Quaternion.Euler(0, instantaneousAttackAngle, 0);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 2f * rotationSpeed * Time.deltaTime);
        }
    }

    private void SwingMeleeWeapon(string animationName)
    {
        CancelCurrentSwing();
        currentSwingingCoroutine = StartCoroutine(SwingCoroutine(animationName, 0.25f));
    }

    private IEnumerator SwingCoroutine(string animationName, float animationFadeSpeed)
    {
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

        player.IsAttacking = false;
    }

    private void CancelCurrentSwing()
    {
        if (currentSwingingCoroutine == null) return;

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
