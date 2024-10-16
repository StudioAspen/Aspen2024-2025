using KBCore.Refs;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class InputReader : MonoBehaviour
{
    [Header("References")]
    [SerializeField, Self] private PlayerInput playerInput;

    [HideInInspector] public UnityEvent<Vector3> Move;
    [HideInInspector] public UnityEvent Jump;
    [HideInInspector] public UnityEvent SprintHold;
    [HideInInspector] public UnityEvent SprintRelease;
    [HideInInspector] public UnityEvent Dash;
    [HideInInspector] public UnityEvent Attack1;
    [HideInInspector] public UnityEvent Attack1Charged;
    [HideInInspector] public UnityEvent Attack1Charging;
    [HideInInspector] public UnityEvent Attack2;
    [HideInInspector] public UnityEvent Attack2Charged;
    [HideInInspector] public UnityEvent Attack2Charging;
    [HideInInspector] public UnityEvent Attack1n2; //Simultaneous button press

    [HideInInspector] public UnityEvent<PlayerActions> OnPlayerActionInput;

    public Vector3 MoveDirection { get; private set; }

    [Header("Hold Thresholds")]
    [SerializeField] private float sprintReleaseToDashThreshold = 0.25f;
    [SerializeField] private float attackReleaseThreshold = 0.25f;
    private float sprintHoldTimer;
    private float attack1HoldTimer;
    private float attack2HoldTimer;
    private bool a1N2; // Attack 1+2 bool

    private void OnValidate()
    {
        this.ValidateRefs();
    }

    private void OnDisable()
    {
        MoveDirection = Vector3.zero;
    }

    private void Update()
    {
        UpdateInputs();
        HandleHoldInputs();

        InvokeInputs();
    }

    private void InvokeInputs()
    {
        if(MoveDirection.sqrMagnitude > 0) Move?.Invoke(MoveDirection);

        if (playerInput.actions["Jump"].WasPressedThisFrame())
        {
            Jump?.Invoke();
        }
    }

    private void UpdateInputs()
    {
        Vector2 movementInput = playerInput.actions["Movement"].ReadValue<Vector2>();

        MoveDirection = new Vector3(movementInput.x, 0, movementInput.y);
    }

    private void HandleHoldInputs()
    {
        // Timers
        if (playerInput.actions["Sprint"].IsPressed())
        {
            sprintHoldTimer += Time.unscaledDeltaTime;
            SprintHold?.Invoke();
        }
        if (playerInput.actions["Attack1"].IsPressed()) attack1HoldTimer += Time.unscaledDeltaTime;
        if (playerInput.actions["Attack2"].IsPressed()) attack2HoldTimer += Time.unscaledDeltaTime;
        
        // Simultaneous Button Check
        if (playerInput.actions["Attack2"].IsPressed() && playerInput.actions["Attack1"].IsPressed()) a1N2 = true;

        // Charging
        if(attack1HoldTimer > attackReleaseThreshold && !a1N2) Attack1Charging?.Invoke();
        if(attack2HoldTimer > attackReleaseThreshold && !a1N2) Attack2Charging?.Invoke();

        // Releasing
        if (playerInput.actions["Sprint"].WasReleasedThisFrame())
        {
            if (sprintHoldTimer < sprintReleaseToDashThreshold)
            {
                Dash?.Invoke();
            }
            SprintRelease?.Invoke();
            sprintHoldTimer = 0f;
        }

        if (playerInput.actions["Attack1"].WasReleasedThisFrame())
        {
            if (attack1HoldTimer < attackReleaseThreshold && a1N2) // attack 1+2 swing
            {
                Debug.Log("Attack now!");
                Attack1n2?.Invoke();
                a1N2 = false;
            }

            else if (attack1HoldTimer < attackReleaseThreshold && !a1N2) // regular swing
            {
                Attack1?.Invoke();
            }
            else if (attack1HoldTimer > attackReleaseThreshold && !a1N2)// charged swing
            {
                Attack1Charged?.Invoke();
            }
            attack1HoldTimer = 0f;
            
        }

        if (playerInput.actions["Attack2"].WasReleasedThisFrame())
        {
            if (attack2HoldTimer < attackReleaseThreshold && a1N2) // attack 1+2 swing
            {
                // Debug.Log("Attack now!");
                Attack1n2?.Invoke();
                a1N2 = false;
            }
            
            else if (attack2HoldTimer < attackReleaseThreshold && !a1N2) // regular swing
            {
                Attack2?.Invoke();
            }
            
            else if (attack2HoldTimer > attackReleaseThreshold && !a1N2) // charged swing
            {
                Attack2Charged?.Invoke();
            }
            attack2HoldTimer = 0f;
        }
    }
}
