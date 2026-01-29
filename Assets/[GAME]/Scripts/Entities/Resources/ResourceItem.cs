using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ResourceItem : AbstractInteractable, IEntity
{
    public EntityStateMachine StateMachine { get; private set; }
    public ResourceData Data { get; private set; }
    public bool IndicatorShowed { get; private set; } = true;

    public void Init(ResourceData resourceData)
    {
        Data = resourceData;

        Collider.enabled = false;

        Init();

        InitStateMachine();
    }

    private void InitStateMachine()
    {
        var stateMachine = new EntityStateMachine();
        var spawnState = new ResourceSpawnState(stateMachine, this);
        var waterState = new ResourceWaterState(stateMachine, this, Data);
        var magnetState = new ResourceMagnetState(stateMachine, this, Data);
        var collectState = new ResourceCollectState(stateMachine, this, Data);

        stateMachine.AddState(spawnState);
        stateMachine.AddState(waterState);
        stateMachine.AddState(magnetState);
        stateMachine.AddState(collectState);
        StateMachine = stateMachine;
    }

    public void Spawn(Vector3 targetPos)
    {
        ResourceSpawnState state = StateMachine.GetState<ResourceSpawnState>();
        state.SetTargetPosition(targetPos);
        StateMachine.SetState<ResourceSpawnState>();
    }

    public override bool CanInteract(PlayerCharacter player)
    {
        bool isAvailable = base.CanInteract(player);

        if (isAvailable == false)
            return false;
        else
            return StateMachine.CurrentEntityState is ResourceWaterState;
    }

    protected override void ExecuteDone()
    {
        StateMachine.SetState<ResourceMagnetState>();
    }

    private void Update()
    {
        if (StateMachine != null)
        {
            StateMachine.Update();
        }
    }

    private void FixedUpdate()
    {
        if (StateMachine != null)
        {
            StateMachine.FixedUpdate();
        }
    }

    private void OnDestroy()
    {
        if (StateMachine != null)
        {
            StateMachine.Disable();
        }
    }
}