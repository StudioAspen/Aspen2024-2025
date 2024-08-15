using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InputReader : MonoBehaviour
{
    [HideInInspector] public Vector3 MoveDirection;
    [HideInInspector] public bool Jump;
    [HideInInspector] public bool SprintHold;
    [HideInInspector] public bool SprintRelease;
    [HideInInspector] public bool Attack;

    private void OnDisable()
    {
        MoveDirection = Vector3.zero;
        Jump = false;
        SprintHold = false;
        SprintRelease = false;
        Attack = false;
    }

    private void Update()
    {
        UpdateInputs();
    }

    private void UpdateInputs()
    {
        MoveDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical")).normalized;
        Jump = Input.GetKeyDown(KeyCode.Space);
        SprintHold = Input.GetKey(KeyCode.LeftShift);
        SprintRelease = Input.GetKeyUp(KeyCode.LeftShift);
        Attack = Input.GetKeyDown(KeyCode.Mouse0);
    }
}
