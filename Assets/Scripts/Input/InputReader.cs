using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InputReader : MonoBehaviour
{
    public Vector3 MoveDirection { get; private set; }
    public bool Jump { get; private set; }
    public bool SprintHold { get; private set; }
    public bool SprintRelease { get; private set; }
    public bool BasicAttack { get; private set; }

    private void OnDisable()
    {
        MoveDirection = Vector3.zero;
        Jump = false;
        SprintHold = false;
        SprintRelease = false;
        BasicAttack = false;
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
        BasicAttack = Input.GetKeyDown(KeyCode.Mouse0);
    }
}
