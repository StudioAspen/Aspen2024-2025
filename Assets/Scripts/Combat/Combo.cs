using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Combo", order = 1)]
public class Combo : ScriptableObject
{
    [field: SerializeField] public List<PlayerActions> Actions { get; private set; } = new List<PlayerActions>();

    public bool Equals(List<PlayerActions> other)
    {
        if(other.Count != Actions.Count) return false;

        for(int i = 0; i < Actions.Count; i++)
        {
            if(!Actions[i].Equals(other[i])) return false;
        }

        return true;
    }
}

public enum PlayerActions
{
    Jump,
    Dash,
    BasicAttack
}