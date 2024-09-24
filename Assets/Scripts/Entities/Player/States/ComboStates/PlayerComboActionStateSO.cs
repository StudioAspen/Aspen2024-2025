using RPGCharacterAnims.Lookups;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ComboState", order = 1)]

public class PlayerComboActionStateSO : PlayerBaseState
{
    private PlayerCombat playerCombat;

    [field: SerializeField] public ComboData ComboData { get; private set; }

    private void OnValidate()
    {
#if UNITY_EDITOR
        ComboData.SetName(Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(this)));
#endif
    }

    public void Init(Entity entity, PlayerCombat playerCombat)
    {
        base.Init(entity);
        this.playerCombat = playerCombat;
    }

    public override void OnEnter()
    {
        playerCombat.Weapon.ClearEnemiesHitList();

        playerCombat.Weapon.SetDamageRange(ComboData.ComboDamageRange);

        player.ReplaceComboAnimationClip(ComboData.ComboClip);
        player.SetComboAnimationSpeed(ComboData.ComboClipAnimationSpeed);
        player.TransitionToAnimation("Combo", 0.05f);

        playerCombat.IsAnimationPlaying = true;
        player.IsAttacking = true;
        player.ApplyRootMotion = ComboData.AttackHasRootMotion;

        player.ApplyRotationToNextMovement();
    }

    public override void OnExit()
    {
        playerCombat.IsAnimationPlaying = false;
        player.IsAttacking = false;
        player.ApplyRootMotion = false;
        playerCombat.DisableWeaponTriggers();

        player.InstantlySetSpeed(0f);
    }

    public override void Update()
    {
        player.ApplyGravity();

        if(!playerCombat.IsAnimationPlaying) player.ChangeState(player.DefaultState);

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
}
