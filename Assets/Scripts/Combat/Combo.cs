using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Combo", order = 1)]
public class Combo : ScriptableObject
{
    [field: SerializeField] public string Name { get; private set; } = "";
    [field: SerializeField] public AnimationClip AnimationClip { get; private set; }
    [field: SerializeField] public List<PlayerActions> Actions { get; private set; } = new List<PlayerActions>();

    public bool IsIn(List<PlayerActions> other) // Checks to see if this combo is in the list of player actions
    {
        if(Actions.Count > other.Count) return false;

        for(int i = 0; i < other.Count; i++)
        {
            int matches = 0;

            for(int j = 0; j < Actions.Count; j++)
            {
                if (i + j >= other.Count) break;

                PlayerActions otherIndex = other[i + j];
                PlayerActions currIndex = Actions[j];

                if (otherIndex == currIndex) matches++;
            }

            if(matches == Actions.Count) return true;
        }

        return false;
    }
}

public enum PlayerActions
{
    Jump,
    Dash,
    BasicAttack
}