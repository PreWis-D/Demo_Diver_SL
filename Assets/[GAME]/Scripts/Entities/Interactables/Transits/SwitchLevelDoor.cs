using DG.Tweening;
using UnityEngine;

public class SwitchLevelDoor : AbstractInteractable
{
    [SerializeField] private Transform _door;
    [SerializeField] private Vector3 _targetRotateDoor;

    private Tween _tween;
    private bool _isOpennig;

    public bool Enabled { get; private set; }
    public bool IndicatorShowed { get; private set; } = true;

    public override void Execute()
    {
        base.Execute();

        if (_isOpennig)
            return;

        _isOpennig = true;
        _tween.Kill();
        _tween = _door.transform.DOLocalRotate(_targetRotateDoor, FloatParameter.MaxValue).SetEase(Ease.Linear);
    }

    public override void Interrupt()
    {
        base.Interrupt();

        if (_isOpennig == false)
            return;

        _isOpennig = false;
        _tween.Kill();
        _tween = _door.transform.DOLocalRotate(Vector3.zero, 0.1f).SetEase(Ease.Linear);
        FloatParameter.SetToMin();
    }

    private void OnDestroy()
    {
        _tween.Kill();
    }

    protected override void ExecuteDone()
    {
        SL.Get<EventProcessingService>().ChangeGameStateInvoke(GameState.SwitchScene);
    }
}