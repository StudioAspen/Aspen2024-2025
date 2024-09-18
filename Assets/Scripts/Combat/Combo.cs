using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Combo", order = 1)]
public class Combo : ScriptableObject
{
    [field: SerializeField] public string Name { get; private set; } = "";
    [field: SerializeReference] public PlayerBaseComboActionState ComboActionState { get; private set; }
    [field: SerializeField] public AnimationClip AnimationClip { get; private set; }
    [field: Range(0.25f, 5f)]
    [field: SerializeField] public float AnimationSpeed { get; private set; } = 1f;
    [field: SerializeField] public Vector2Int BaseDamageRange { get; private set; } = new Vector2Int(10, 15);
    [field: SerializeField] public List<PlayerActions> Actions { get; private set; } = new List<PlayerActions>();

    private void OnValidate()
    {
#if UNITY_EDITOR
        Name = Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(this));
#endif
    }

    /// <summary>
    /// Checks to see if the given combo (starting from the front) is potentially in the other combo
    /// </summary>
    /// <param name="givenComboList"></param>
    /// <param name="otherComboList"></param>
    /// <returns></returns>
    public static bool IsPotentiallyIn(List<PlayerActions> givenComboList, List<PlayerActions> otherComboList) 
    {
        if(otherComboList.Count > givenComboList.Count) return false;

        List<PlayerActions> subList = givenComboList.GetRange(0, Mathf.Min(givenComboList.Count, otherComboList.Count));

        return IsIn(subList, otherComboList);
    }

    /// <summary>
    /// Checks to see if a given combo is in the other combo
    /// </summary>
    /// <param name="givenComboList"></param>
    /// <param name="otherComboList"></param>
    /// <returns></returns>
    public static bool IsIn(List<PlayerActions> givenComboList, List<PlayerActions> otherComboList) 
    {
        if (givenComboList.Count > otherComboList.Count) return false;

        for (int i = 0; i < otherComboList.Count; i++)
        {
            int matches = 0;

            for (int j = 0; j < givenComboList.Count; j++)
            {
                if (i + j >= otherComboList.Count) break;

                PlayerActions otherAction = otherComboList[i + j];
                PlayerActions currAction = givenComboList[j];

                if (otherAction == currAction) matches++;
            }

            if (matches == givenComboList.Count) return true;
        }

        return false;
    }

    /// <summary>
    /// Returns the single action combo from a list of combos. If it doesn't exist returns null.
    /// </summary>
    /// <param name="combos"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    public static PlayerBaseComboActionState GetSingleActionCombo(List<PlayerBaseComboActionState> combos, PlayerActions action)
    {
        foreach(PlayerBaseComboActionState combo in combos)
        {
            if (combo.Actions.Count == 1 && combo.Actions.Contains(action)) return combo;
        }

        return null;
    }

    /// <summary>
    /// Returns the combo with the most number of actions given a list of combos
    /// </summary>
    /// <param name="combos"></param>
    /// <returns></returns>
    public static PlayerBaseComboActionState GetLongestCombo(List<PlayerBaseComboActionState> combos)
    {
        if (combos.Count == 0) return null;
        if (combos.Count == 1) return combos[0];

        PlayerBaseComboActionState result = null;

        foreach (PlayerBaseComboActionState combo in combos)
        {
            if (result == null)
            {
                result = combo;
                continue;
            }

            if (combo.Actions.Count > result.Actions.Count)
            {
                result = combo;
            }
        }

        return result;
    }
}
