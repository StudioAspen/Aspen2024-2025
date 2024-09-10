using KBCore.Refs;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class InputReader : MonoBehaviour
{
    [Header("References")]
    [SerializeField, Self] private PlayerInput playerInput;

    [HideInInspector] public UnityEvent<Vector3> Move;
    [HideInInspector] public UnityEvent Jump;
    [HideInInspector] public UnityEvent SprintHold;
    [HideInInspector] public UnityEvent SprintRelease;
    [HideInInspector] public UnityEvent Attack1Hold;
    [HideInInspector] public UnityEvent Attack1Release;
    [HideInInspector] public UnityEvent Attack2Hold;
    [HideInInspector] public UnityEvent Attack2Release;

    [HideInInspector] public UnityEvent<PlayerActions> OnPlayerActionInput;

    public Vector3 MoveDirection { get; private set; }

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

        InvokeInputs();
    }

    private void InvokeInputs()
    {
        if(MoveDirection.sqrMagnitude > 0) Move?.Invoke(MoveDirection);

        if (playerInput.actions["Jump"].WasPressedThisFrame())
        {
            Jump?.Invoke();
        }

        if (playerInput.actions["Sprint"].IsPressed())
        {
            SprintHold?.Invoke();
        }

        if (playerInput.actions["Sprint"].WasReleasedThisFrame())
        {
            SprintRelease?.Invoke();
        }

        if (playerInput.actions["Attack1"].IsPressed())
        {
            Attack1Hold?.Invoke();
        }

        if (playerInput.actions["Attack1"].WasReleasedThisFrame())
        {
            Attack1Release?.Invoke();
        }

        if (playerInput.actions["Attack2"].IsPressed())
        {
            Attack2Hold?.Invoke();
        }

        if (playerInput.actions["Attack2"].WasReleasedThisFrame())
        {
            Attack2Release?.Invoke();
        }
    }

    private void UpdateInputs()
    {
        Vector2 movementInput = playerInput.actions["Movement"].ReadValue<Vector2>();

        MoveDirection = new Vector3(movementInput.x, 0, movementInput.y);
    }
}

public enum PlayerActions
{
    Jump,
    Dash,
    Attack1,
    ChargeAttack1,
    Attack2,
    ChargeAttack2
}
