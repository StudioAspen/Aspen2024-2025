using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewAugment", menuName = "AOE Augment", order = 2)]
public class AoeAugment : Augment
{
    [field: Header("[Augment Attributes]")]
    [field: SerializeField] public float Radius { get; private set; } = 0.0f;
    [field: SerializeField] public float Strength { get; private set; } = 0.0f;

    public override void Activate()
    {
        base.Activate();
        Debug.Log("Explosion Activated");
    }

    // Implementing Evan's abstract from his Augment to not return error
    public override AugmentBranch GetBranch() { return AugmentBranch.NONE; } // chose a random branch name existed
    public override int GetLevel() { return 1; }
}
