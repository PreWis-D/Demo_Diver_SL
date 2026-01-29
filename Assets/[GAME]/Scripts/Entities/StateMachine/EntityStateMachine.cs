using System;
using System.Collections.Generic;

public class EntityStateMachine
{
    public event Action<EntityState, IEntity> OnEnterState;
    public event Action<EntityState, IEntity> OnExitState;

    public EntityState CurrentEntityState { get; set; }
    private Dictionary<Type, EntityState> _states = new Dictionary<Type, EntityState>();

    public void AddState(EntityState entityState)
    {
        if (_states.ContainsKey(entityState.GetType()))
        {
            return;
        }
        _states.Add(entityState.GetType(), entityState);
    }

    public void SetState<T>() where T : EntityState
    {
        var type = typeof(T);
        if (CurrentEntityState != null && CurrentEntityState.GetType() == type)
        {
            return;
        }

        if (_states.TryGetValue(type, out var next))
        {
            CurrentEntityState?.Exit();
            OnExitState?.Invoke(CurrentEntityState, CurrentEntityState?.Entity);
            CurrentEntityState = next;
            CurrentEntityState?.Enter();
            OnEnterState?.Invoke(CurrentEntityState, CurrentEntityState?.Entity);
        }
    }

    public T GetState<T>() where T : EntityState
    {
        var type = typeof(T);
        if (_states.TryGetValue(type, out var next))
        {
            return (T)next;
        }

        return null;
    }

    public void Update()
    {
        CurrentEntityState?.Update();
    }

    public void Disable()
    {
        CurrentEntityState?.Exit();
    }

    public void FixedUpdate()
    {
        CurrentEntityState?.FixedUpdate();
    }
}