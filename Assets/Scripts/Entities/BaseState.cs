using UnityEngine;

public abstract class BaseState
{
    public BaseState(Entity entity) { }

    public abstract void OnEnter();
    public abstract void OnExit();
    public abstract void Update();
    public abstract void FixedUpdate();
}
