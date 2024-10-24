using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AugmentManager : MonoBehaviour
{
    private AugmentBranch currentBranch = AugmentBranch.NONE;
    private int unlockedLevel = 0;
    private int lastLevelAdded = 0;

    [SerializeField] private GameObject player;

    public bool AddAugment<T>()
        where T : Augment, new()
    {
        T augment = new T();

        if (currentBranch == AugmentBranch.NONE)
        {
            currentBranch = augment.GetBranch();
        }
        else if (augment.GetBranch() != currentBranch)
        {
            return false;
        }

        if (augment.GetLevel() > unlockedLevel || augment.GetLevel() != lastLevelAdded + 1)
        {
            return false;
        }

        player.AddComponent<T>();
        lastLevelAdded = augment.GetLevel();
        return true;
    }

    public void IncrementLevel()
    {
        unlockedLevel++;
    }
}
