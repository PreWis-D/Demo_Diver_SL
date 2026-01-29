using UnityEngine;

public class IntParameter : ObservableParameter<int>
{
    public IntParameter(int maxValue, int currentValue, float criticalThreshold = 0.2F) 
        : base(maxValue, currentValue, criticalThreshold) { }

    public override int ClampValue(int value)
    {
        return Mathf.Clamp(value, 0, _maxValue);
    }

    public override bool IsAtMinValue()
    {
        return _currentValue <= 0;
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
        return (float)_currentValue / _maxValue;
    }

    public void IncreaseValue(int value)
    {
        CurrentValue += value;
        CurrentValue = IsAtMaxValue() ? MaxValue : CurrentValue;
    }

    public void DecreaseValue(int value)
    {
        CurrentValue -= value;
        CurrentValue = IsAtMinValue() ? 0 : CurrentValue;
    }

    public void OverrideValue(int value)
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