using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Augment : MonoBehaviour
{
    public abstract AugmentBranch GetBranch();
    public abstract int GetLevel();
}
