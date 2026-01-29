using UnityEngine;

public class FloatParameter : ObservableParameter<float>
{
    public FloatParameter(float maxValue, float currentValue, float criticalThreshold = 0.2F)
    : base(maxValue, currentValue, criticalThreshold) { }

    public override float ClampValue(float value)
    {
        return Mathf.Clamp(value, 0f, _maxValue);
    }

    public override bool IsAtMinValue()
    {
        return _currentValue <= 0f;
    }

    public override bool IsAtMaxValue()
    {
        return _currentValue >= _maxValue;
    }

    public override bool IsCritical()
    {
        return _currentValue <= _maxValue * _criticalThreshold;
    }

    public override float GetPercentage()
    {
        return _currentValue / _maxValue;
    }

    public void IncreaseValue(float value)
    {
        CurrentValue += value;
        CurrentValue = IsAtMaxValue() ? MaxValue : CurrentValue;
    }

    public void DecreaseValue(float value)
    {
        CurrentValue -= value;
        CurrentValue = IsAtMinValue() ? 0 : CurrentValue;
    }

    public void OverrideValue(float value)
    {
        if (value > MaxValue)
            value = MaxValue;
        else if (value < 0)
            value = 0;

        CurrentValue = value;
    }

    public void SetToMax()
    {
        CurrentValue = _maxValue;
    }

    public void SetToMin()
    {
        CurrentValue = 0;
    }
}