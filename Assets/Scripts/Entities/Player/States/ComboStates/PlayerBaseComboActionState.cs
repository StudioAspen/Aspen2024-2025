using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class PlayerBaseComboActionState : PlayerBaseState
{
    [field:Header("Base Action: Settings")]
    public string Name { get; private set; } = "";
    [SerializeField] protected private AnimationClip animationClip;
    
    [SerializeField] [Range(0.25f, 5f)] protected private float animationSpeed = 1f;
    [SerializeField] protected private Vector2Int baseDamageRange = new Vector2Int(10, 15);
    [field: SerializeField] public List<PlayerActions> Actions { get; private set; } = new List<PlayerActions>();

    private void OnValidate()
    {
#if UNITY_EDITOR
        Name = Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(this));
#endif
    }

    public override void Init(Entity entity)
    {
        base.Init(entity);
    }

    public override void OnEnter()
    {
        Debug.Log("Entering " + GetType().ToString() + " State");
    }

    public override void OnExit()
    {

    }

    public override void Update()
    {

    }

    public override void FixedUpdate()
    {

    }
}
