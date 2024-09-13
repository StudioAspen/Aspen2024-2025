using UnityEngine;

[CreateAssetMenu]
public class GlobalPhysicsSettings : ScriptableObject
{
    [field: SerializeField] public float Gravity { get; private set; } = -20f;
    [field: SerializeField] public float GroundedYVelocity { get; private set; } = -5f;
    [field: SerializeField] public float FallingStartingYVelocity { get; private set; } = 0f;
}