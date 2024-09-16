using UnityEngine;

public abstract class BaseState
{
    public int Priority {  get; private set; }

    public BaseState(Entity entity, int prio)
    {
        Priority = prio;
    }

    public abstract void OnEnter();
    public abstract void OnExit();
    public abstract void Update();
    public abstract void FixedUpdate();
}
