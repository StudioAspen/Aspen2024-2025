using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ComboData", order = 1)]
public class ComboDataSO : ScriptableObject
{
    [field: Header("[Combo Data]")]
    [field: SerializeField] public string ComboName { get; private set; } = "";
    [field: SerializeField] public List<PlayerActions> ComboInputs { get; private set; } = new List<PlayerActions>();
    [field: SerializeField] public AnimationClip ComboClip { get; private set; }
    [field: SerializeField] [field: Range(0.01f, 5f)] public float ComboClipAnimationSpeed { get; private set; } = 1f;

    [field: Header("[Filter Options]")]
    [field: SerializeField] public bool AttackHasRootMotion { get; private set; } = true;
    //[field: SerializeField] public bool IsAirCombo { get; private set; }
    //[field: SerializeField] public bool IsSprintCombo { get; private set; }

    [field: Header("[Hit Options]")]
    [field: SerializeField] public Vector2Int ComboDamageRange { get; private set; } = new Vector2Int(10, 15);

    private void OnValidate()
    {
#if UNITY_EDITOR
        ComboName = Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(this));
#endif
    }

    public void SetName(string name)
    {
        ComboName = name;
    }

    /// <summary>
    /// Checks to see if the given combo (starting from the front) is potentially in the other combo
    /// </summary>
    /// <param name="givenComboList"></param>
    /// <param name="otherComboList"></param>
    /// <returns></returns>
    public static bool IsPotentiallyIn(List<PlayerActions> givenComboList, List<PlayerActions> otherComboList)
    {
        if (otherComboList.Count > givenComboList.Count) return false;

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
    public static ComboDataSO GetSingleActionCombo(List<ComboDataSO> combos, PlayerActions action)
    {
        foreach (ComboDataSO combo in combos)
        {
            if (combo.ComboInputs.Count == 1 && combo.ComboInputs.Contains(action)) return combo;
        }

        return null;
    }

    /// <summary>
    /// Returns the combo with the most number of actions given a list of combos
    /// </summary>
    /// <param name="combos"></param>
    /// <returns></returns>
    public static ComboDataSO GetLongestCombo(List<ComboDataSO> combos)
    {
        if (combos.Count == 0) return null;
        if (combos.Count == 1) return combos[0];

        ComboDataSO result = null;

        foreach (ComboDataSO combo in combos)
        {
            if (result == null)
            {
                result = combo;
                continue;
            }

            if (combo.ComboInputs.Count > result.ComboInputs.Count)
            {
                result = combo;
            }
        }

        return result;
    }
}
