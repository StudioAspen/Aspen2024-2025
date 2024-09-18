using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ComboStates/Swing", order = 1)]
public class PlayerSwingActionState : PlayerBaseComboActionState
{
    [Header("Swing Action: Settings")]
    [SerializeField] private float stepForwardDistance = 0.5f;
    [SerializeField] private float stepForwardSpeed = 3f;
    private float stepForwardDuration => stepForwardDistance / stepForwardSpeed; // d = v * t
    private float stepForwardTimer;

    public override void Init(Entity entity)
    {
        base.Init(entity);
    }

    public override void OnEnter()
    {
        Debug.Log("Entering " + GetType().ToString() + " State");

        player.ReplaceComboAnimationClip(animationClip);
        player.SetComboAnimationSpeed(animationSpeed);
        player.TransitionToAnimation("Combo", 0.05f);

        player.ApplyRotationToNextMovement();

        stepForwardTimer = 0f;
    }

    public override void OnExit()
    {
        player.SetGroundedSpeed(0f);
    }

    public override void Update()
    {
        if(stepForwardTimer < stepForwardDuration * 2f) stepForwardTimer += Time.deltaTime;

        player.RotateToTargetRotation();

        if (stepForwardTimer < stepForwardDuration)
        {
            player.ApplyRotationToNextMovement();
            player.SetGroundedSpeed(stepForwardSpeed);
            player.GroundedMove();
        }
    }

    public override void FixedUpdate()
    {

    }
}