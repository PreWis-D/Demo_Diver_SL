using DG.Tweening;
using System;
using UnityEngine;

public class ResourceProducerElement : MonoBehaviour, IDamagable
{
    [SerializeField] private Transform _view;
    [SerializeField] private ResourcesGenerator _rewardGenerator;
    [SerializeField] private float _delayResourceSpawn = 0.2f;

    private HitFx _hitVFXPrefab;
    private HitFx _dieFXPrefab;
    private Sequence _sequence;
    private FloatParameter _floatParameter;
    private ResourceSpawner _resourceSpawner;
    private ResourceType _type;
    private float _radius = 1;
    private bool _isCanResourcesGenerated;

    public Transform HitTargetPoint { get; set; }
    public Transform Target { get; private set; }
    public float Resistance { get; private set; } = 100;
    public int Level { get; private set; }
    public bool IsDied { get; private set; }

    public event Action<IDamagable> ElementDied;
    public event Action<ResourceItem> ResourceGenerated;

    public void Init(
        float health,
        Transform hitPoint,
        HitFx hitVFX,
        HitFx dieFX,
        ResourceType type,
        float resistance,
        bool isCanResourcesGenerated)
    {
        HitTargetPoint = hitPoint;
        _hitVFXPrefab = hitVFX;
        _dieFXPrefab = dieFX;
        _type = type;
        _isCanResourcesGenerated = isCanResourcesGenerated;
        Resistance = resistance;

        _resourceSpawner = new ResourceSpawner(_rewardGenerator, transform, _radius, _delayResourceSpawn);
        _floatParameter = new FloatParameter(health, health);
    }

    public void SetTarget(Transform target)
    {
        Target = target;
    }

    public void TakeDamage(float damage)
    {
        if (_floatParameter.IsAtMinValue())
            return;

        _floatParameter.DecreaseValue(damage);
        HitFx hitVFX = PoolManager.GetPool(_hitVFXPrefab, transform.position);
        hitVFX.Init(_type);
        var direction = (Target.transform.position - transform.position).normalized;
        hitVFX.transform.rotation = Quaternion.LookRotation(-direction);

        if (_floatParameter.IsAtMinValue())
            Died();
        else
            PlayerDamageEffect();
    }

    public void Died()
    {
        IsDied = true;

        HitFx dieFX = PoolManager.GetPool(_dieFXPrefab, transform.position);
        dieFX.Init(_type);

        GenerateResources(transform.position);

        ElementDied?.Invoke(this);
        gameObject.SetActive(false);
    }

    public void GenerateResources(Vector3 position)
    {
        if (_isCanResourcesGenerated)
            _resourceSpawner.SpawnResources();
    }

    private void PlayerDamageEffect()
    {
        if (_view)
        {
            _sequence.Kill();
            _sequence = DOTween.Sequence();
            _sequence.SetEase(Ease.Linear);
            _sequence.Append(_view.transform.DOLocalRotate(new Vector3(0, 0, -5f), 0.05f));
            _sequence.Append(_view.transform.DOLocalRotate(new Vector3(0, 0, 0), 0.05f));
            _sequence.Append(_view.transform.DOLocalRotate(new Vector3(0, 0, 5), 0.05f));
            _sequence.Append(_view.transform.DOLocalRotate(new Vector3(0, 0, 0), 0.05f));
        }
    }

    private void OnDestroy()
    {
        _sequence.Kill();
        _resourceSpawner.OnDestroy();
    }
}