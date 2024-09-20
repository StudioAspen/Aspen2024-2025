using KBCore.Refs;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Player: Debug UI")]
    [SerializeField] private TMP_Text inputsText;
    [SerializeField] private TMP_Text comboText;

    [Header("References")]
    [SerializeField, Self] private InputReader input;
    [SerializeField, Self] private Player player;
    [SerializeField, Self] private Animator animator;

    [field: Header("Settings")]
    [field: SerializeField] public WeaponHandler Weapon { get; private set; }
    [HideInInspector] public bool IsAnimationPlaying;

    [Header("Combo")]
    [SerializeField] private float comboListenDuration = 1f;
    private float comboListenTimer;
    private List<PlayerActions> currentComboList = new List<PlayerActions>();
    private List<PlayerComboActionStateSO> potentialCombos = new List<PlayerComboActionStateSO>();
    private List<PlayerComboActionStateSO> predictedCombos = new List<PlayerComboActionStateSO>();

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
        HandleComboList();
        HandleWeaponTriggers();

        DebugUICombos();
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

        if (attack2HoldTimer > attackReleaseThreshold)
        {
            player.IsChargingAttack = true;

            // play the animation matching the predicted potential combo
        }
    }

    private void HandleAttack2ReleaseInput()
    {
        if (!player.CanAttack) return;
        if (player.IsAttacking) return;

        if (attack2HoldTimer < attackReleaseThreshold) // regular swing 2
        {
            input.OnPlayerActionInput?.Invoke(PlayerActions.Attack2);
        }
        else // charged swing 2
        {
            input.OnPlayerActionInput?.Invoke(PlayerActions.ChargeAttack2);
        }

        player.IsChargingAttack = false;
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
    }

    private void HandleOnPlayerActionInput(PlayerActions incomingAction)
    {
        comboListenTimer = 0;

        currentComboList.Add(incomingAction);

        GenerateComboLists();

        AttemptToExecuteACombo(incomingAction);
    }

    private void AttemptToExecuteACombo(PlayerActions incomingAction)
    {
        PlayerComboActionStateSO comboToExecute = null;

        if (predictedCombos.Count == 0) // if new action doesn't create any valid combos
        {
            currentComboList.Clear();
            currentComboList.Add(incomingAction);

            GenerateComboLists();

            comboToExecute = ComboData.GetSingleActionCombo(Weapon.Combos, incomingAction);
            if (comboToExecute != null)
            {
                ExecuteCombo(comboToExecute);
            }
        }
        else
        {
            comboToExecute = ComboData.GetLongestCombo(potentialCombos);

            if (comboToExecute != null)
            {
                ExecuteCombo(comboToExecute);
            }
        }

        //PrintComboLists();
    }

    private void ExecuteCombo(PlayerComboActionStateSO comboState)
    {
        comboState.Init(player, this);
        player.ChangeState(comboState);

        comboText.text = "Combo: " + comboState.ComboData.ComboName;
    }

    private void GenerateComboLists()
    {
        potentialCombos = new List<PlayerComboActionStateSO>();
        predictedCombos = new List<PlayerComboActionStateSO>();
        foreach (PlayerComboActionStateSO weaponCombo in Weapon.Combos)
        {
            if (ComboData.IsIn(weaponCombo.ComboData.ComboInputs, currentComboList)) potentialCombos.Add(weaponCombo);
            if (ComboData.IsPotentiallyIn(weaponCombo.ComboData.ComboInputs, currentComboList)) predictedCombos.Add(weaponCombo);
        }
    }

    private void DebugUICombos()
    {
        string inputs = "Inputs: ";

        for (int i = 0; i < currentComboList.Count; i++)
        {
            inputs += currentComboList[i].ToString();
            if (i != currentComboList.Count - 1) inputs += ",";
            inputs += " ";
        }

        inputsText.text = inputs;

        if (currentComboList.Count == 0) comboText.text = "Combo: ";
    }

    private void PrintComboLists()
    {
        string result = "Current Combo: { ";

        for (int i = 0; i < currentComboList.Count; i++)
        {
            result += currentComboList[i].ToString();
            if (i != currentComboList.Count - 1) result += ",";
            result += " ";
        }

        result += "}\nPotential Combos: { ";

        for (int i = 0; i < potentialCombos.Count; i++)
        {
            result += potentialCombos[i].ComboData.ComboName;
            if (i != potentialCombos.Count - 1) result += ",";
            result += " ";
        }

        result += "}\nPredicted Combos: { ";

        for (int i = 0; i < predictedCombos.Count; i++)
        {
            result += potentialCombos[i].ComboData.ComboName;
            if (i != predictedCombos.Count - 1) result += ",";
            result += " ";
        }

        result += "}";

        Debug.Log(result);
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

    private void HandleWeaponTriggers()
    {
        if (!player.IsAttacking) DisableWeaponTriggers();
    }

    public void EnableWeaponTriggers()
    {
        Weapon.EnableTriggers();
    }

    public void DisableWeaponTriggers()
    {
        Weapon.DisableTriggers();
    }

    public void FinishAnimation()
    {
        IsAnimationPlaying = false;
    }
}
