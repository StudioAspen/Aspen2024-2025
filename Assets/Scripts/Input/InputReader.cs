using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class InputReader : MonoBehaviour
{
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

    private void OnEnable()
    {
        
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

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump?.Invoke();
        }

        if (Input.GetKey(KeyCode.LeftShift)) 
        {
            SprintHold?.Invoke();
        }

        if (Input.GetKeyUp(KeyCode.LeftShift)) 
        {
            SprintRelease?.Invoke();
        }

        if (Input.GetKey(KeyCode.Mouse0))
        {
            Attack1Hold?.Invoke();
        }

        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            Attack1Release?.Invoke();
        }

        if (Input.GetKey(KeyCode.Mouse1))
        {
            Attack2Hold?.Invoke();
        }

        if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            Attack2Release?.Invoke();
        }
    }

    private void UpdateInputs()
    {
        MoveDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical")).normalized;
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
