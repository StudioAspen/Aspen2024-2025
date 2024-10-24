using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Augment : MonoBehaviour // public class Augment : ScriptableObject (Ambrose)
{
    // Ambrose
    [field: Header("[Augment Data]")]
    [field: SerializeField] public string Name { get; private set; } = "";
    [field: SerializeField] public Sprite Icon { get; private set; }
    [field: SerializeField] public Augments Requirement { get; private set; }
    public bool isActive = false;

    public virtual void Activate()
    {
        Debug.Log("Augment Activated");
    }

    // Evan
    public abstract AugmentBranch GetBranch();
    public abstract int GetLevel();
}
