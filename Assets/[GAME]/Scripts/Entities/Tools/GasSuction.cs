using System;
using UnityEngine;
using UnityEngine.UI;

public class GasSuction : AbstractHandItem
{
    [SerializeField] private Transform _tip;
    [SerializeField] private Image[] _workloadBars;

    [field: SerializeField] public Transform LeftHand { get; private set; }
    [field: SerializeField] public Transform RightHand { get; private set; }
    [field: SerializeField] public Transform Tube { get; private set; }

    private float _startMaxWorkload;
    private PlayerCharacter _playerCharacter;
    private float _detectRadius = 0.1f;
    private bool _isDetectProducer;

    public override HandItemType Type => HandItemType.GasSuction;

    public FloatParameter FloatParameter { get; private set; }

    public override void Init(HandItemData itemData)
    {
        base.Init(itemData);

        GasSuctionData data = itemData as GasSuctionData;

        _startMaxWorkload = data.StartMaxWorkload;
        FloatParameter = new FloatParameter(_startMaxWorkload, 0);
        FloatParameter.SetToMin();
        FloatParameter.ValueChanged += OnValueChanged;
    }

    public override bool CanInteract()
    {
        if (Input.IsFiring == false)
            Interrupt();

        return View.gameObject.activeSelf && Input.IsFiring;
    }

    public void Interact()
    {
        if (_isDetectProducer)
            return;

        Collider[] colliders = Physics.OverlapSphere(
            _tip.transform.position,
            _detectRadius
        );

        foreach (var collider in colliders)
        {
            GasProducer gasProducer = collider.GetComponent<GasProducer>();
            if (gasProducer && gasProducer.FloatParameter.IsAtMinValue() == false)
            {
                if (_playerCharacter == null)
                    _playerCharacter = SL.Get<CharactersService>().GetPlayerCharacter();

                _isDetectProducer = true;
                PlayerInteractGasProducerState state = _playerCharacter.StateMachine.GetState<PlayerInteractGasProducerState>();
                state.SetGasProducer(gasProducer);
                _playerCharacter.StateMachine.SetState<PlayerInteractGasProducerState>();
            }
        }
    }

    public void Interrupt()
    {
        _isDetectProducer = false;
    }

    public void AddGas()
    {
        FloatParameter.IncreaseValue(Time.deltaTime);
    }

    private void OnValueChanged(float arg1, float arg2)
    {
        for (int i = 0; i < _workloadBars.Length; i++)
            _workloadBars[i].fillAmount = FloatParameter.GetPercentage();
    }

    protected override void HandleVisible()
    {
        View.gameObject.SetActive(Input.IsAim);
    }

    private void OnDestroy()
    {
        FloatParameter.ValueChanged -= OnValueChanged;
    }
}