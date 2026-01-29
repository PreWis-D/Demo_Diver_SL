using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WorkloadIndicator : MonoBehaviour
{
    [SerializeField] private RectTransform _view;
    [SerializeField] private Image _fill;
    [SerializeField] private TMP_Text _maxText;
    [SerializeField] private float _animateDurationStep = 0.15f;

    private IntParameter _intParameter;
    private Sequence _sequence;
    private Color _currentColor = Color.white;

    public void Init(IntParameter intParameter)
    {
        _intParameter = intParameter;
        _intParameter.ValueChanged += OnValueChanged;
        SL.Get<EventProcessingService>().CantAddResource += OnCantAddResource;

        _maxText.gameObject.SetActive(false);
    }

    private void OnValueChanged(int currentValue, int maxValue)
    {
        _currentColor = Color.green;
        float percentage = _intParameter.GetPercentage();
        _fill.fillAmount = percentage;
        _maxText.gameObject.SetActive(false);
        bool isCritical = percentage > _intParameter.CriticalThreshold;

        if (isCritical)
        {
            _currentColor = Color.yellow;
        }
        else if (_intParameter.IsAtMaxValue())
        {
            _currentColor = Color.red;
            _maxText.gameObject.SetActive(true);
        }

        _fill.color = _currentColor;
    }

    private void OnCantAddResource(ResourceData data)
    {
        _sequence.Kill();
        _sequence = DOTween.Sequence();
        _sequence.Append(_view.DOLocalMove(new Vector3(10, 0, 0), _animateDurationStep).SetEase(Ease.Linear));
        _sequence.Join(_fill.DOColor(Color.red, _animateDurationStep).SetEase(Ease.Linear));
        _sequence.Append(_view.DOLocalMove(new Vector3(0, 0, 0), _animateDurationStep).SetEase(Ease.Linear));
        _sequence.Append(_view.DOLocalMove(new Vector3(-10, 0, 0), _animateDurationStep).SetEase(Ease.Linear));
        _sequence.Append(_view.DOLocalMove(new Vector3(0, 0, 0), _animateDurationStep).SetEase(Ease.Linear));
        _sequence.Join(_fill.DOColor(_currentColor, _animateDurationStep).SetEase(Ease.Linear));
    }

    private void OnDestroy()
    {
        _intParameter.ValueChanged -= OnValueChanged;
        SL.Get<EventProcessingService>().CantAddResource -= OnCantAddResource;
    }
}