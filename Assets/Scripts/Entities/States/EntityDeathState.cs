using UnityEngine;

public class EntityDeathState : BaseState
{
    private Entity entity;

    private float timer;

    public EntityDeathState(Entity entity) : base(entity)
    {
        this.entity = entity;
    }

    public override void OnEnter()
    {
        entity.DefaultTransitionToAnimation("Death");

        timer = 0f;
    }

    public override void OnExit()
    {

    }

    public override void Update()
    {
        timer += Time.deltaTime;

        if(timer > 1f)
        {
            entity.DestroyEntity();
        }
    }

    public override void FixedUpdate()
    {

    }
}