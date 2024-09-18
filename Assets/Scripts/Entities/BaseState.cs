using Unity.VisualScripting;
using UnityEngine;

public abstract class BaseState : ScriptableObject
{
    public static S CreateState<S>(Entity entity) where S : BaseState
    {
        // Create an instance of the ScriptableObject
        S state = ScriptableObject.CreateInstance<S>();

        // Initialize any fields or properties here if needed
        // For example, you might call an Init method if you define one in derived classes
        state.Init(entity);

        return state;
    }

    public virtual void Init(Entity entity)
    {
        
    }

    public abstract void OnEnter();
    public abstract void OnExit();
    public abstract void Update();
    public abstract void FixedUpdate();
}
