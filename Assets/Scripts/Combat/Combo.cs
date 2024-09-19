using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Combo", order = 1)]
public class Combo : ScriptableObject
{
    [field: SerializeField] public string Name { get; private set; } = "";
    [field: SerializeReference] public PlayerComboActionStateSO ComboActionState { get; private set; }
    [field: SerializeField] public AnimationClip AnimationClip { get; private set; }
    [field: Range(0.25f, 5f)]
    [field: SerializeField] public float AnimationSpeed { get; private set; } = 1f;
    [field: SerializeField] public Vector2Int BaseDamageRange { get; private set; } = new Vector2Int(10, 15);
    [field: SerializeField] public List<PlayerActions> Actions { get; private set; } = new List<PlayerActions>();

    private void OnValidate()
    {
#if UNITY_EDITOR
        if (Name == null) Name = "";
        Name = Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(this));
#endif
    }

   
}
