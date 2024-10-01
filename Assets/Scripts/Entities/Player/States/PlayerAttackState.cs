using RPGCharacterAnims.Lookups;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class PlayerAttackState : PlayerBaseState
{
    private PlayerCombat playerCombat;

    public ComboDataSO ComboData { get; private set; }

    public PlayerAttackState(Player player) : base(player)
    {
        this.player = player;
    }

    public override void OnEnter()
    {
        playerCombat.Weapon.ClearEnemiesHitList();

        playerCombat.Weapon.SetDamageRange(ComboData.ComboDamageRange);

        player.ReplaceComboAnimationClip(ComboData.ComboClip);
        player.SetComboAnimationSpeed(ComboData.ComboClipAnimationSpeed);
        player.TransitionToAnimation("Combo", 0.05f);

        playerCombat.IsAnimationPlaying = true;
        player.ApplyRootMotion = ComboData.AttackHasRootMotion;

        player.ApplyRotationToNextMovement();
    }

    public override void OnExit()
    {
        playerCombat.IsAnimationPlaying = false;
        player.ApplyRootMotion = false;
        playerCombat.DisableWeaponTriggers();

        player.InstantlySetSpeed(0f);
    }

    public override void Update()
    {
        player.ApplyGravity();

        if (!playerCombat.IsAnimationPlaying) player.ChangeState(player.DefaultState);

        if (player.MoveDirection != Vector3.zero) player.ApplyRotationToNextMovement();

        player.RotateToTargetRotation();
        player.AccelerateToSpeed(0f);
        player.InstantlySetSpeed(player.GetGroundedVelocity().magnitude);
        player.GroundedMove();

        player.RotateToTargetRotation();
    }

    public override void FixedUpdate()
    {

    }

    public void SetCombo(PlayerCombat playerCombat, ComboDataSO comboData)
    {
        this.playerCombat = playerCombat;
        ComboData = comboData;
    }
}

