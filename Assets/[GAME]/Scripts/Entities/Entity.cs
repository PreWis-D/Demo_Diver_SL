using UnityEngine;

public abstract class Entity : MonoBehaviour, IEntity
{
    public Transform Transform => transform;
    public EntityStateMachine StateMachine { get; protected set; }

    public virtual void InitState(EntityStateMachine stateMachine)
    {
        StateMachine = stateMachine;
    }

    protected virtual void Update()
    {
        if (StateMachine != null)
        {
            StateMachine.Update();
        }
    }

    protected virtual void FixedUpdate()
    {
        if (StateMachine != null)
        {
            StateMachine.FixedUpdate();
        }
    }

    protected virtual void OnDestroy()
    {
        if (StateMachine != null)
        {
            StateMachine.Disable();
        }
    }
}
