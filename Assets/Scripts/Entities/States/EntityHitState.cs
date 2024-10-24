using System.Collections;
using UnityEngine;

public class EntityHitState : BaseState
{
    private Entity entity;

    private float timer = 0f;

    public EntityHitState(Entity entity) : base(entity)
    {
        this.entity = entity;
    }

    public override void OnEnter()
    {
        entity.DefaultTransitionToAnimation("Hit");

        timer = 0f;

        entity.SetSpeedModifier(0);
    }

    public override void OnExit()
    {
        
    }

    public override void Update()
    {
        timer += Time.deltaTime;
        if (timer > 0.5f) entity.ChangeState(entity.DefaultState);
    }

    public override void FixedUpdate()
    {

    }
}
