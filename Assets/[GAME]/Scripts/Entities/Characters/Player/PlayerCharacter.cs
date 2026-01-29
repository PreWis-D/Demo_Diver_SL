using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : Entity
{
    [field: SerializeField] public PlayerAnimator Animator { get; private set; }
    [field: SerializeField] public RigidbodyModel RigidbodyModel { get; private set; }
    [field: SerializeField] public PlayerInventory PlayerInventory { get; private set; }

    [field: SerializeField] public Transform IndicatorPoint { get; private set; }
    [field: SerializeField] public Rigidbody Rigidbody { get; private set; }
    [field: SerializeField] public Transform View { get; private set; }
    [field: SerializeField] public Collider Collider { get; private set; }
    [field: SerializeField] public Transform AimTarget { get; private set; }

    private PlayerData _playerData;
    private PlayerInteractHandler _interactHandler;
    private PlayerAimHandler _aimHandler;
    private VacuumController _vacuumController;
    private PlayerHandItemHandler _handItemHandler;

    public void Init()
    {
        _playerData = SL.Get<CharactersService>().GetPlayerData();

        Animator.Init(this, _playerData);
        PlayerInventory.Init();
        _interactHandler = new PlayerInteractHandler(this, _playerData);
        _aimHandler = new PlayerAimHandler(this, AimTarget);
        _vacuumController = new VacuumController(this, _playerData);
        _handItemHandler = new PlayerHandItemHandler(this, _playerData);

        InitStateMachine();

        List<Collider> colliders = RigidbodyModel.GetColliders();

        foreach (var collider in colliders)
            Physics.IgnoreCollision(collider, Collider, true);
    }

    public void ChangeVacuumActiveState(bool isActive)
    {
        _vacuumController.ChangeActiveState(isActive);
    }

    private void InitStateMachine()
    {
        var stateMachine = new EntityStateMachine();
        var waterState = new PlayerWaterState(stateMachine, this);
        var aimState = new PlayerAimInWaterState(stateMachine, this);
        var groundState = new PlayerGroundState(stateMachine, this);
        var switchLayerZ = new PlayerSwitchVectorZState(stateMachine, this);
        var ladderToGroundState = new PlayerLadderClimbState(stateMachine, this);
        var jumpState = new PlayerJumpState(stateMachine, this);
        var interactGasProducerState = new PlayerInteractGasProducerState(stateMachine, this);

        stateMachine.AddState(waterState);
        stateMachine.AddState(aimState);
        stateMachine.AddState(groundState);
        stateMachine.AddState(switchLayerZ);
        stateMachine.AddState(ladderToGroundState);
        stateMachine.AddState(jumpState);
        stateMachine.AddState(interactGasProducerState);
        InitState(stateMachine);

        CheckStartState(stateMachine);
    }

    private void CheckStartState(EntityStateMachine stateMachine)
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 0.1f);

        foreach (Collider collider in colliders)
        {
            if (collider.gameObject.layer == 3)
                stateMachine.SetState<PlayerGroundState>();
            else if (collider.gameObject.layer == 4)
                stateMachine.SetState<PlayerWaterState>();
        }
    }

    protected override void Update()
    {
        base.Update();

        _interactHandler.Update();
        _aimHandler.Update();
        _interactHandler.Update();
        _vacuumController.Update();
        _handItemHandler.Update();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        _playerData.Cancel();
        _vacuumController.OnDestroy();
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (Application.isPlaying)
        {
            _vacuumController.OnDrawGizmosSelected();
            _interactHandler.OnDrawGizmosSelected();
        }
    }
#endif
}