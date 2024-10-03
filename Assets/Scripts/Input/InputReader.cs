using KBCore.Refs;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class InputReader : MonoBehaviour
{
    [Header("References")]
    [SerializeField, Self] private PlayerInput playerInput;
    [SerializeField, Anywhere] private TMP_Text bufferedInputsDebugText;

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

    [HideInInspector] public UnityEvent<PlayerActions> OnSuccessfulPlayerActionInput;

    public Vector3 MoveDirection { get; private set; }

    [Header("Input Buffering")]
    [SerializeField] private float bufferDuration = 0.25f;
    private float bufferTimer;
    private List<PlayerActions> bufferedInputsList = new List<PlayerActions>();

    [Header("Hold Thresholds")]
    [SerializeField] private float sprintReleaseToDashThreshold = 0.25f;
    [SerializeField] private float attackReleaseThreshold = 0.25f;
    private float sprintHoldTimer;
    private float attack1HoldTimer;
    private float attack2HoldTimer;

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

        HandleInputBuffering();
    }

    private void InvokeInputs()
    {
        if(MoveDirection.sqrMagnitude > 0) Move?.Invoke(MoveDirection);

        if (playerInput.actions["Jump"].WasPressedThisFrame())
        {
            Jump?.Invoke();
            OnAttemptedPlayerActionInput(PlayerActions.JUMP);
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

        // Charging
        if(attack1HoldTimer > attackReleaseThreshold) Attack1Charging?.Invoke();
        if(attack2HoldTimer > attackReleaseThreshold) Attack2Charging?.Invoke();

        // Releasing
        if (playerInput.actions["Sprint"].WasReleasedThisFrame())
        {
            if (sprintHoldTimer < sprintReleaseToDashThreshold)
            {
                Dash?.Invoke();
                OnAttemptedPlayerActionInput(PlayerActions.DASH);
            }
            SprintRelease?.Invoke();
            sprintHoldTimer = 0f;
        }

        if (playerInput.actions["Attack1"].WasReleasedThisFrame())
        {
            if (attack1HoldTimer < attackReleaseThreshold) // regular swing
            {
                Attack1?.Invoke();
                OnAttemptedPlayerActionInput(PlayerActions.ATTACK1);
            }
            else // charged swing
            {
                Attack1Charged?.Invoke();
                OnAttemptedPlayerActionInput(PlayerActions.CHARGEDATTACK1);
            }
            attack1HoldTimer = 0f;
        }

        if (playerInput.actions["Attack2"].WasReleasedThisFrame())
        {
            if (attack2HoldTimer < attackReleaseThreshold) // regular swing
            {
                Attack2?.Invoke();
                OnAttemptedPlayerActionInput(PlayerActions.ATTACK2);
            }
            else // charged swing
            {
                Attack2Charged?.Invoke();
                OnAttemptedPlayerActionInput(PlayerActions.CHARGEDATTACK2);
            }
            attack2HoldTimer = 0f;
        }
    }

    private void OnAttemptedPlayerActionInput(PlayerActions incomingAction)
    {
        bufferedInputsList.Add(incomingAction);

        bufferTimer = 0f;
    }

    private void HandleInputBuffering()
    {
        DebugBufferedInputs();

        if (bufferedInputsList.Count > 0) bufferTimer += Time.unscaledDeltaTime;

        if(bufferTimer > bufferDuration)
        {
            bufferedInputsList.Clear();
            bufferTimer = 0f;
        }
    }

    public PlayerActions GetNextInputFromBuffer()
    {
        if (bufferedInputsList.Count == 0) return PlayerActions.NONE;

        return bufferedInputsList[0];
    }

    public void RemoveFirstInputFromBuffer()
    {
        if (bufferedInputsList.Count == 0) return;
        bufferedInputsList.RemoveAt(0);
        bufferTimer = 0f;
    }

    private void DebugBufferedInputs()
    {
        string text = "";

        for (int i = 0; i < bufferedInputsList.Count; i++)
        {
            text += bufferedInputsList[i].ToString();
            if (i != bufferedInputsList.Count - 1) text += ",";
            text += " ";
        }

        bufferedInputsDebugText.text = $"Buffer: {text}";
    }
}
