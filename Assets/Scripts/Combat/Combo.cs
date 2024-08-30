using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Combo", order = 1)]
public class Combo : ScriptableObject
{
    [field: SerializeField] public List<PlayerActions> PrimaryCombo { get; private set; } = new List<PlayerActions>();
}

public enum PlayerActions
{
    Jump,
    Dash,
    BasicAttack
}