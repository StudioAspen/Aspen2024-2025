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
        Debug.Log("Entering " + GetType().ToString() + " State");

        playerCombat.Weapon.ClearEnemiesHitList();

        playerCombat.Weapon.SetDamageRange(ComboData.ComboDamageRange);

        player.ReplaceComboAnimationClip(ComboData.ComboClip);
        player.SetComboAnimationSpeed(ComboData.ComboClipAnimationSpeed);
        player.TransitionToAnimation("Combo", 0.05f);

        playerCombat.IsAnimationPlaying = true;
        player.IsAttacking = true;

        player.ApplyRotationToNextMovement();
    }

    public override void OnExit()
    {
        playerCombat.IsAnimationPlaying = false;
        player.IsAttacking = false;
        playerCombat.DisableWeaponTriggers();

        player.SetGroundedSpeed(0f);
    }

    public override void Update()
    {
        if(!playerCombat.IsAnimationPlaying) player.ChangeState(player.DefaultState);

        if (player.MoveDirection != Vector3.zero) player.ApplyRotationToNextMovement();

        player.RotateToTargetRotation();
    }

    public override void FixedUpdate()
    {

    }
}
