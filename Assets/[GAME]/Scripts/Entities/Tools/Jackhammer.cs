using System;
using UnityEngine;

public class Jackhammer : AbstractHandItem
{
    [SerializeField] private LayerMask _layerMask;
    [SerializeField] private Animator _animator;
    [SerializeField] private Transform _tipPoint;
    [field: SerializeField] public Transform IndicatorPoint { get; private set; }
    [field: SerializeField] public Transform LeftHand { get; private set; }
    [field: SerializeField] public Transform RightHand { get; private set; }

    private float _damage;
    private float _startOperatingTimeBeforeOverheating;
    private float _radiusDetectTargets = 0.5f;
    private float _coolingTime;
    private float _currentTime = 0;
    private float _attackTime = 0.1f;
    private float _currentAttackTime;
    private bool _isActive;

    private const string MINING = "IsMining";

    public override HandItemType Type => HandItemType.Jackhammer;

    public FloatParameter FloatParameter { get; private set; }
    public bool IsOverheat { get; private set; }

    public override void Init(HandItemData itemData)
    {
        base.Init(itemData);

        JackhammerData data = itemData as JackhammerData;
        _damage = data.Damage;
        _startOperatingTimeBeforeOverheating = data.StartOperatingTimeBeforeOverheating;
        _coolingTime = data.CoolingTime;

        FloatParameter = new FloatParameter(_startOperatingTimeBeforeOverheating, _startOperatingTimeBeforeOverheating, 0.4f);
    }

    public override bool CanInteract()
    {
        if (Input.IsFiring == false)
            Interrupt();

        return View.gameObject.activeSelf && Input.IsFiring && IsOverheat == false;
    }

    public void Interact()
    {
        _isActive = true;
        _animator.SetBool(MINING, true);
    }

    public void Interrupt()
    {
        _animator.SetBool(MINING, false);
        _isActive = false;
    }

    protected override void Update()
    {
        base.Update();

        if (IsOverheat)
        {
            Interrupt();
            HandleOverheatState();
            return;
        }

        if (_isActive)
            Work();
        else
            Rest();
    }

    private void HandleOverheatState()
    {
        _currentTime += Time.deltaTime;

        if (_currentTime > _coolingTime)
        {
            IsOverheat = false;
            _currentTime = 0;
            FloatParameter.SetToMax();
        }
    }

    private void Work()
    {
        if (FloatParameter.IsAtMinValue() == false)
            FloatParameter.DecreaseValue(Time.deltaTime);
        else
            IsOverheat = true;

        if (_isActive == false)
            return;

        _currentAttackTime += Time.deltaTime;

        if (_currentAttackTime > _attackTime)
        {
            _currentAttackTime = 0;

            Collider[] colliders = Physics.OverlapSphere(
            transform.position,
            _radiusDetectTargets,
            _layerMask
        );

            foreach (var collider in colliders)
            {
                ResourceProducerElement element = collider.GetComponent<ResourceProducerElement>();
                if (element.IsDied == false)
                {
                    element.SetTarget(transform);
                    element.TakeDamage(_damage);
                }
            }
        }
    }

    private void Rest()
    {
        if (FloatParameter.IsAtMaxValue() == false)
            FloatParameter.IncreaseValue(Time.deltaTime);
    }

    protected override void HandleVisible()
    {
        View.gameObject.SetActive(Input.IsAim);
    }
}