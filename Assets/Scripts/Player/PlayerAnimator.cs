using KBCore.Refs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [Header("References")]
    [SerializeField, Self] private PlayerController player;
    [SerializeField, Self] private Animator animator;

    [Header("Settings")]
    [SerializeField] private float attackAnimationSpeedMultiplier = 3f;

    private void Update()
    {
        UpdateAnimationParameters();
    }

    private void UpdateAnimationParameters()
    {
        //animator.SetFloat("MovementSpeed", player.CurrentMovementSpeed / player.MaxSpeed);

        animator.SetBool("IsGrounded", player.IsGrounded);
        animator.SetBool("IsJumping", player.IsJumping);
        animator.SetFloat("InAirTimer", player.InAirTimer);

        animator.SetFloat("AttackAnimationSpeedMultiplier", attackAnimationSpeedMultiplier);
    }
}
