using UnityEngine;

public class Skill : ScriptableObject
{
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] public Sprite IconSprite { get; private set; }
    [field: SerializeField] public SkillBehaviour SkillPrefab { get; private set; }
    [TextArea(15, 20)]
    [field: SerializeField] public string Description;
}

public class SkillBehaviour : MonoBehaviour
{
    protected private PlayerController player;

    public void Init(PlayerController player)
    {
        this.player = player;
    }
}