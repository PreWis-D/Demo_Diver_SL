using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class PlayerInteractGasProducerState : EntityState
{
    private PlayerCharacter _player;
    private PlayerData _playerData;
    private Rigidbody _rigidbody;
    private GasProducer _gasProducer;
    private InputProcessingService _input;
    private GasSuction _gasSuction;
    private Sequence _sequence;

    private bool _isActive;
    private Vector3 _startPosition;

    public PlayerInteractGasProducerState(EntityStateMachine stateMachine, IEntity entity) : base(stateMachine, entity)
    {
        _player = entity as PlayerCharacter;
        _playerData = SL.Get<CharactersService>().GetPlayerData();
        _rigidbody = _player.GetComponent<Rigidbody>();
        _gasSuction = _player.PlayerInventory.GetHandItem(HandItemType.GasSuction) as GasSuction;
        _input = SL.Get<InputProcessingService>();
    }

    public void SetGasProducer(GasProducer gasProducer)
    {
        _gasProducer = gasProducer;
    }

    public override void Enter()
    {
        base.Enter();

        _rigidbody.linearVelocity = Vector3.zero;
        _rigidbody.isKinematic = true;
        _playerData.ChangeAnimationBlandSpeed(0);
        Connect().Forget();
    }

    private async UniTask Connect()
    {
        if (_isActive)
            return;

        bool isAnimate = true;
        _startPosition = _gasSuction.Tube.transform.localPosition;

        _sequence.Kill();
        _sequence = DOTween.Sequence();
        _sequence.Append(_player.transform.DOMove(_gasProducer.ConnectPlayerPoint.position, 0.25f).SetEase(Ease.Linear));
        _sequence.Append(_gasSuction.Tube.transform.DOMove(_gasProducer.ConnectToolPoint.position, 0.75f).SetEase(Ease.InBack));
        _sequence.OnComplete(() => isAnimate = false);

        while (isAnimate)
            await UniTask.Yield();

        _gasProducer.ChangeVisibleGasFX(false);
        _isActive = true;
    }

    public override void Update()
    {
        if (_input.IsFiring == false)
            Unconnect().Forget();

        if (_isActive == false)
            return;

        base.Update();

        if (_gasProducer.FloatParameter.IsAtMinValue() || _gasSuction.FloatParameter.IsAtMaxValue())
            Unconnect().Forget();

        _gasProducer.Interact();
        _gasSuction.AddGas();
    }

    private async UniTask Unconnect()
    {
        if (_isActive == false)
            return;

        _gasProducer.Interrupt();
        bool isAnimate = true;
        _isActive = false;

        _sequence.Kill();
        _sequence = DOTween.Sequence();
        _sequence.Append(_gasSuction.Tube.transform.DOLocalMove(_startPosition, 0.75f).SetEase(Ease.OutBack));
        _sequence.OnComplete(() => isAnimate = false);

        while (isAnimate)
            await UniTask.Yield();

        _gasProducer.ChangeVisibleGasFX(true);
        _player.StateMachine.SetState<PlayerWaterState>();
    }

    public override void Exit()
    {
        base.Exit();

        _rigidbody.isKinematic = false;
        _sequence.Kill();
    }
}