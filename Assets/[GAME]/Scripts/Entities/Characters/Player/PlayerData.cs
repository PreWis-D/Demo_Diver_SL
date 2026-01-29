using Cysharp.Threading.Tasks;
using System;
using System.Linq;
using System.Threading;
using UnityEngine;

public class PlayerData
{
    public FloatParameter HealthParam { get; private set; }
    public FloatParameter StaminaParam { get; private set; }
    public IntParameter WorkloadParam { get; private set; }
    public MovementData MovementData { get; private set; }
    public VacuumData VacuumData { get; private set; }
    public float MoveSpeed { get; private set; }
    public float SprintSpeed { get; private set; }
    public float CurrentSpeed { get; private set; }
    public float AnimationBlendSpeed { get; private set; }
    public bool IsStaminaReload { get; private set; }
    public bool IsAim { get; private set; }
    public LocationSpace LocationSpace { get; private set; }
    public float MoveSpeedInWater { get; internal set; }

    private float _speedBalanceModifier = 100;
    private LayerZState _layerZState = LayerZState.Middle;

    private CancellationTokenSource _cancellationTokenSource;

    public PlayerData(PlayerConfig config)
    {
        MovementData = config.MovementData;
        VacuumData = config.VacuumData;

        float health = GetParamValue(config, ParamType.Health);
        HealthParam = new FloatParameter(health, health);

        float stamina = GetParamValue(config, ParamType.Stamina);
        StaminaParam = new FloatParameter(stamina, stamina, 0.4f);

        int liftingCapacity = (int)GetParamValue(config, ParamType.Workload);
        WorkloadParam = new IntParameter(liftingCapacity, 0, 0.8f);
        WorkloadParam.SetToMin();

        MoveSpeed = GetParamValue(config, ParamType.Speed) * _speedBalanceModifier;
        SprintSpeed = GetParamValue(config, ParamType.SprintSpeed) * _speedBalanceModifier;
        MoveSpeedInWater = GetParamValue(config, ParamType.SpeedInWater) * _speedBalanceModifier;

        SL.Get<EventProcessingService>().ResourceChanged += OnResourcesChanged;
    }

    private float GetParamValue(PlayerConfig config, ParamType type)
    {
        return config.Params.FirstOrDefault(c => c.Type == type).Value;
    }

    public void HandleSprint(bool isRun)
    {
        if (isRun && IsStaminaReload == false && StaminaParam.IsAtMinValue() == false)
        {
            StaminaParam.DecreaseValue(0.01f);
            CurrentSpeed = SprintSpeed;
        }
        else
        {
            if (StaminaParam.IsAtMinValue())
            {
                _cancellationTokenSource?.Cancel();
                _cancellationTokenSource = new CancellationTokenSource();
                ReloadStamina(_cancellationTokenSource.Token).Forget();
            }

            StaminaParam.IncreaseValue(0.01f);
            CurrentSpeed = MoveSpeed;
        }
    }

    public void ChangeAnimationBlandSpeed(float speed)
    {
        AnimationBlendSpeed = speed;
    }

    public void ChangeLocationSpace(LocationSpace location)
    {
        LocationSpace = location;
    }

    public void ChangeAimState(bool inAim)
    {
        IsAim = inAim;
    }

    public float SwitchLayerZState()
    {
        switch (_layerZState)
        {
            case LayerZState.Middle:
                _layerZState = LayerZState.Back;
                break;
            case LayerZState.Back:
                _layerZState = LayerZState.Middle;
                break;
        }

        return _layerZState switch
        {
            LayerZState.Middle => Constants.MIDDLE_LAYER,
            LayerZState.Back => Constants.BACK_LAYER,
            LayerZState.Forward => Constants.FORWARD_LAYER,
        };
    }

    public void Cancel()
    {
        _cancellationTokenSource?.Cancel();
    }

    private async UniTask ReloadStamina(CancellationToken token)
    {
        IsStaminaReload = true;
        while (StaminaParam.IsCritical())
            await UniTask.Yield(cancellationToken: token);
        IsStaminaReload = false;
    }

    private void OnResourcesChanged(ResourceData data)
    {
        if (HasResource(data.Type))
            WorkloadParam.IncreaseValue(data.LoadValue);
    }

    private bool HasResource(ResourceType resourceType)
    {
        return resourceType switch
        {
            ResourceType.Metall => true,
            ResourceType.Plastic => true,
            _ => false
        };
    }
}