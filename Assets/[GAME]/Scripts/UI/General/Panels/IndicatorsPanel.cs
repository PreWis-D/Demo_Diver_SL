using System;
using UnityEngine;

public class IndicatorsPanel : AbstractPanel
{
    [SerializeField] private RectTransform _indicatorsContainer;
    [SerializeField] private RectTransform _aimIndicatorContainer;
    [SerializeField] private WorkloadIndicator _workloadIndicator;

    private GeneralComponentsService _generalComponentsService;
    private EventProcessingService _eventsService;
    private PlayerCharacter _playerCharacter;
    private PlayerData _playerData;
    private InteractIndicator _interactIndicator;

    public override PanelType Type => PanelType.Indicators;

    public override void Init()
    {
        base.Init();

        _playerCharacter = SL.Get<CharactersService>().GetPlayerCharacter();
        _playerData = SL.Get<CharactersService>().GetPlayerData();
        _generalComponentsService = SL.Get<GeneralComponentsService>();
        _eventsService = SL.Get<EventProcessingService>();

        _eventsService.InteractableFounded += OnInteractableFounded;

        CreatePlayerStaminaIndicator();
        CreateInteractIndicator();
        CreateAimIndicator();
        CreateJackhummerIndicator();

        _workloadIndicator.Init(_playerData.WorkloadParam);
    }

    private void CreatePlayerStaminaIndicator()
    {
        var staminaIndicatorGO = Instantiate(
            _generalComponentsService.GetPrefab(PrefabType.StaminaIndicator), _indicatorsContainer);
        StaminaIndicator staminaIndicator = staminaIndicatorGO.GetComponent<StaminaIndicator>();
        staminaIndicator.Init(_playerData.StaminaParam, _playerCharacter.IndicatorPoint, Canvas);
    }

    private void CreateAimIndicator()
    {
        var aimIndicatorGO = Instantiate(
            _generalComponentsService.GetPrefab(PrefabType.AimIndicator), _aimIndicatorContainer);
        AimIndicator aimIndicator = aimIndicatorGO.GetComponent<AimIndicator>();
        aimIndicator.Init(Canvas);
    }

    private void CreateInteractIndicator()
    {
        var indicatorPrefab = _generalComponentsService.GetPrefab(PrefabType.InteractIndicator);
        var interactIndicatorGO = PoolManager.GetPool(indicatorPrefab, _indicatorsContainer);
        _interactIndicator = interactIndicatorGO.GetComponent<InteractIndicator>();
        _interactIndicator.Init(new FloatParameter(1, 0), _playerCharacter.IndicatorPoint, Canvas);
    }

    private void CreateJackhummerIndicator()
    {
        var jackhummerIndicatorGO = Instantiate(
            _generalComponentsService.GetPrefab(PrefabType.JackhummerIndicator), _indicatorsContainer);
        JackhummerIndicator jackhummerIndicator = jackhummerIndicatorGO.GetComponent<JackhummerIndicator>();
        Jackhammer jackhammer = _playerCharacter.PlayerInventory.GetHandItem(HandItemType.Jackhammer) as Jackhammer;
        jackhummerIndicator.Init(jackhammer, Canvas);
    }

    private void OnInteractableFounded(IInteractable interactable, bool isFounded)
    {
        _interactIndicator.UpdateInteractable(interactable, isFounded);
    }

    private void OnDestroy()
    {
        _eventsService.InteractableFounded -= OnInteractableFounded;
    }
}