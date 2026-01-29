using DG.Tweening;
using UnityEngine;

public class Chest : AbstractInteractable
{
    [SerializeField] private ResourcesGenerator _rewardGenerator;
    [SerializeField] private Transform _spawnPoint;

    [Header("Animation open")]
    [SerializeField] private Transform _cap;
    [SerializeField] private Vector3 _targetRotate;
    [SerializeField] private float _animateDuration;

    [SerializeField] private float _radius = 1.5f;
    [SerializeField] private float _popHeight = 2f;
    [SerializeField] private float _delaySpawn = 0.2f;

    private Tween _tween;
    private ResourceSpawner _resourceSpawner;

    public bool IndicatorShowed { get; private set; } = true;

    public void Start()
    {
        _resourceSpawner = new ResourceSpawner(_rewardGenerator, _spawnPoint, _radius, _delaySpawn);
    }

    public override void Interrupt()
    {
        FloatParameter.SetToMin();
    }

    protected override void ExecuteDone()
    {
        AnimateOpen();
    }

    private void AnimateOpen()
    {
        Collider.enabled = false;

        _tween.Kill();
        _tween = _cap.transform.DOLocalRotate(_targetRotate, _animateDuration).SetEase(Ease.OutBack);
        _tween.OnComplete(() => _resourceSpawner.SpawnResources());
    }

    private void OnDestroy()
    {
        _tween.Kill();
        _resourceSpawner.OnDestroy();
    }
}