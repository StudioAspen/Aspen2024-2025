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
    [field: SerializeField] public Weapon Weapon { get; private set; }
    [HideInInspector] public bool IsAnimationPlaying;

    [Header("Combo")]
    [SerializeField] private float comboListenDuration = 1f;
    private float comboListenTimer;
    private List<PlayerActions> currentComboList = new List<PlayerActions>();
    private List<ComboDataSO> potentialCombos = new List<ComboDataSO>();
    private List<ComboDataSO> predictedCombos = new List<ComboDataSO>();

    private void OnValidate()
    {
        this.ValidateRefs();
    }

    private void OnEnable()
    {
        input.Attack1.AddListener(HandleAttack1Input);
        input.Attack1Charged.AddListener(HandleAttack1ChargedInput);
        input.Attack1Charging.AddListener(HandleAttackChargingInput);
/*        input.Attack2.AddListener(HandleAttack2Input);
        input.Attack2Charged.AddListener(HandleAttack2ChargedInput);
        input.Attack2Charging.AddListener(HandleAttackChargingInput);*/

        input.OnPlayerActionInput.AddListener(HandleOnPlayerActionInput);
    }

    private void OnDisable()
    {
        input.Attack1.RemoveListener(HandleAttack1Input);
        input.Attack1Charged.RemoveListener(HandleAttack1ChargedInput);
        input.Attack1Charging.RemoveListener(HandleAttackChargingInput);
/*        input.Attack2.RemoveListener(HandleAttack2Input);
        input.Attack2Charged.RemoveListener(HandleAttack2ChargedInput);
        input.Attack2Charging.RemoveListener(HandleAttackChargingInput);*/

        input.OnPlayerActionInput.RemoveListener(HandleOnPlayerActionInput);
    }

    private void Update()
    {
        HandleComboList();
        HandleWeaponTriggers();

        DebugUICombos();
    }

    private void HandleAttack1Input()
    {
        if (!player.CanAttack) return;
        if (player.CurrentState == player.PlayerChargeState) return;
        if (player.CurrentState == player.PlayerAttackState) return;

        input.OnPlayerActionInput?.Invoke(PlayerActions.ATTACK1);
    }

    private void HandleAttack1ChargedInput()
    {
        if (!player.CanAttack) return;
        if (player.CurrentState == player.PlayerAttackState) return;

        input.OnPlayerActionInput?.Invoke(PlayerActions.CHARGEDATTACK1);
    }

    private void HandleAttackChargingInput()
    {
        if (!player.CanAttack) return;
        if (player.CurrentState == player.PlayerChargeState) return;
        if (player.CurrentState == player.PlayerAttackState) return;
        if (player.CurrentState == player.PlayerDashState) return;
            
        player.ChangeState(player.PlayerChargeState);
    }

    private void HandleAttack2Input()
    {
        if (!player.CanAttack) return;
        if (player.CurrentState == player.PlayerChargeState) return;
        if (player.CurrentState == player.PlayerAttackState) return;

        input.OnPlayerActionInput?.Invoke(PlayerActions.ATTACK2);
    }

    private void HandleAttack2ChargedInput()
    {
        if (!player.CanAttack) return;
        if (player.CurrentState == player.PlayerAttackState) return;

        input.OnPlayerActionInput?.Invoke(PlayerActions.CHARGEDATTACK2);
    }

    private void HandleComboList()
    {
        if (player.CurrentState != player.PlayerChargeState && player.CurrentState != player.PlayerAttackState && comboListenTimer < comboListenDuration * 2f) comboListenTimer += Time.unscaledDeltaTime;

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
        ComboDataSO comboToExecute = null;

        if (predictedCombos.Count == 0) // if new action doesn't create any valid combos
        {
            currentComboList.Clear();
            currentComboList.Add(incomingAction);

            GenerateComboLists();

            comboToExecute = ComboDataSO.GetSingleActionCombo(Weapon.Combos, incomingAction);
            if (comboToExecute != null)
            {
                ExecuteCombo(comboToExecute);
            }
        }
        else
        {
            comboToExecute = ComboDataSO.GetLongestCombo(potentialCombos);

            if (comboToExecute != null)
            {
                ExecuteCombo(comboToExecute);
            }
        }

        //PrintComboLists();
    }

    private void ExecuteCombo(ComboDataSO combo)
    {
        if (player.CurrentState == player.PlayerSlideState) return;

        player.PlayerAttackState.SetCombo(this, combo);
        player.ChangeState(player.PlayerAttackState);

        comboText.text = "Combo: " + combo.ComboName;
    }

    private void GenerateComboLists()
    {
        potentialCombos = new List<ComboDataSO>();
        predictedCombos = new List<ComboDataSO>();
        foreach (ComboDataSO weaponCombo in Weapon.Combos)
        {
            if (ComboDataSO.IsIn(weaponCombo.ComboInputs, currentComboList)) potentialCombos.Add(weaponCombo);
            if (ComboDataSO.IsPotentiallyIn(weaponCombo.ComboInputs, currentComboList)) predictedCombos.Add(weaponCombo);
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
            result += potentialCombos[i].ComboName;
            if (i != potentialCombos.Count - 1) result += ",";
            result += " ";
        }

        result += "}\nPredicted Combos: { ";

        for (int i = 0; i < predictedCombos.Count; i++)
        {
            result += potentialCombos[i].ComboName;
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
        if (player.CurrentState != player.PlayerAttackState) DisableWeaponTriggers();
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
