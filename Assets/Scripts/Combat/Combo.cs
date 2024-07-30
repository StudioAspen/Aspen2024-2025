using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Combo", order = 1)]
public class Combo : ScriptableObject
{
    [field: SerializeField] public List<MeleeSwing> PrimaryCombo { get; private set; } = new List<MeleeSwing>();
}
