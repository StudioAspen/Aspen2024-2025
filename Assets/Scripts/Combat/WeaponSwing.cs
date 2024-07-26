using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WeaponSwing
{
    [field: SerializeField] public string AnimationClipName { get; private set; }
    [field: SerializeField] public float TimeToStartDamage { get; private set; }
    [field: SerializeField] public float TimeToStopDamage { get; private set; }
}
