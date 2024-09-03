using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class InputReader : MonoBehaviour
{
    [HideInInspector] public UnityEvent<Vector2> Move;
    [HideInInspector] public UnityEvent Jump;
    [HideInInspector] public UnityEvent SprintHold;
    [HideInInspector] public UnityEvent SprintRelease;
    [HideInInspector] public UnityEvent BasicAttack;

    public Vector3 MoveDirection { get; private set; }

    private void OnDisable()
    {
        MoveDirection = Vector3.zero;
    }

    private void Update()
    {
        InvokeInputs();

        UpdateInputs();
    }

    private void InvokeInputs()
    {
        Move?.Invoke(new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")));
        if (Input.GetKeyDown(KeyCode.Space)) Jump?.Invoke();
        if (Input.GetKey(KeyCode.LeftShift)) SprintHold?.Invoke();
        if (Input.GetKeyUp(KeyCode.LeftShift)) SprintRelease?.Invoke();
        if (Input.GetKeyDown(KeyCode.Mouse0)) BasicAttack?.Invoke();
    }

    private void UpdateInputs()
    {
        MoveDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical")).normalized;
    }
}
